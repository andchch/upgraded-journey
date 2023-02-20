using System;
using System.Collections.Generic;
using System.Windows;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;

namespace kursovaya;

/// <summary>
/// Логика взаимодействия для MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Regex mailRegex = new Regex(@"[a-zA-z ]+_[a-zA-z ]+_\d+");
    private readonly AppSettings appSettings;
    private readonly Board[] currentBoards;

    public MainWindow()
    {
        InitializeComponent();
        
        SheetAccess.InitializeConnection();
        currentBoards = UpdateEquipment();
        
        List<Gmail> MailLists = GetAllEmails("email.kursovaya@gmail.com");
        ProcessMail(MailLists, currentBoards);
        
        var manager = new INIManager(Environment.CurrentDirectory + "\\settings.ini");
        appSettings = new AppSettings(manager);
        
        var boardsNames = new List<string>();
        foreach (var board in currentBoards) boardsNames.Add(board.name);
        BoardList.ItemsSource = boardsNames;
    }
    
    private static Board[] UpdateEquipment()
    {
        var isFirst = true;
        var i = 0;

        var sheetdata = SheetAccess.ReadEntries("A:M");
        var boards = new Board[sheetdata.Count - 1];

        foreach (var row in sheetdata)
        {
            if (!isFirst)
            {
                var hasHPS = 0;
                var hasDDR = 0;
                var hasVGA = 0;
                var hasETH = 0;
                if ((string) row[1] == "Да") hasHPS = 1;
                if ((string) row[4] == "Да") hasDDR = 1;
                if ((string) row[9] == "Да") hasVGA = 1;
                if ((string) row[10] == "Да") hasETH = 1;

                boards[i] = new Board((string) row[0], hasHPS, int.Parse((string) row[2]),
                    int.Parse((string) row[3]), hasDDR, int.Parse((string) row[5]),
                    int.Parse((string) row[6]), int.Parse((string) row[7]),
                    int.Parse((string) row[8]), hasVGA, hasETH, (string) row[11],
                    int.Parse((string) row[12]));

                i++;
            }

            isFirst = false;
        }

        return boards;
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow();
        settingsWindow.Owner = this;
        settingsWindow.Show();
    }

    private void CheckBoardButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedBoardName = BoardList.SelectedItem;
        if (selectedBoardName != null)
        {
            foreach (var board in currentBoards)
                if (board.name == selectedBoardName)
                {
                    if (Board.CheckAvaible(board))
                    {
                        var giveWindow = new GiveWindow();

                        if (giveWindow.ShowDialog() == true)
                        {
                            if (giveWindow.Num <= board.numAvailable)
                            {
                                //TODO - Выдача оборудования
                                var row = Array.IndexOf(currentBoards, board) + 2;
                                var newNum = board.numAvailable - giveWindow.Num;
                                
                                SheetAccess.WriteCell($"M{row}", newNum.ToString());
                                SheetAccess.WriteOwner($"A1", giveWindow.Name, giveWindow.Num.ToString());
                                board.numAvailable = newNum;
                            }
                            else
                            {
                                MessageBox.Show("Такого количества нет в наличие", "Внимание",
                                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                        }
                    }
                    else
                    {
                        var suggestedBoard = Board.SuggestEqual(currentBoards, board, appSettings);
                        MessageBox.Show("Этих плат в данный момент нет.\nПредложенная альтернатива:" +
                                        $" {suggestedBoard.name}");
                    }
                }
        }
        else
        {
            MessageBox.Show("Плата не выбрана", "Ошибка", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void Download_Click(object sender, RoutedEventArgs e)
    {
        var format = (string) FileFormat.SelectionBoxItem;
        SheetAccess.Download(format);
    }
    
    private static List<Gmail> GetAllEmails(string HostEmailAddress)
        {
            try
            {
                GmailService GmailService = GmailAccess.GetService();
                List<Gmail> EmailList = new List<Gmail>();
                UsersResource.MessagesResource.ListRequest ListRequest = GmailService.Users
                    .Messages.List(HostEmailAddress);
                ListRequest.LabelIds = "INBOX";
                ListRequest.IncludeSpamTrash = true;
                ListRequest.Q = "is:unread";
                
                ListMessagesResponse ListResponse = ListRequest.Execute();
                
                if (ListResponse != null && ListResponse.Messages != null)
                {
                    foreach (Message Msg in ListResponse.Messages)
                    {
                        GmailAccess.MsgMarkAsRead(HostEmailAddress, Msg.Id);

                        UsersResource.MessagesResource.GetRequest Message = GmailService.Users.Messages
                            .Get(HostEmailAddress, Msg.Id);
                        //Мессадж айди
                        //Console.WriteLine("STEP-1: Message ID:" + Msg.Id);
                        
                        Message MsgContent = Message.Execute();

                        if (MsgContent != null)
                        {
                            string FromAddress = string.Empty;
                            string Date = string.Empty;
                            string Subject = string.Empty;
                            string MailBody = string.Empty;
                            string ReadableText = string.Empty;
                            
                            foreach (var MessageParts in MsgContent.Payload.Headers)
                            {
                                if (MessageParts.Name == "From")
                                {
                                    FromAddress = MessageParts.Value;
                                }
                                else if (MessageParts.Name == "Date")
                                {
                                    Date = MessageParts.Value;
                                }
                                else if (MessageParts.Name == "Subject")
                                {
                                    Subject = MessageParts.Value;
                                }
                            }
                            
                            List<string> FileName = GmailAccess.GetAttachments(HostEmailAddress, Msg.Id,
                                Environment.CurrentDirectory);

                            if (FileName.Count() > 0)
                            {
                                foreach (var EachFile in FileName)
                                {
                                    string[] RectifyFromAddress = FromAddress.Split(' ');
                                    string FromAdd = RectifyFromAddress[RectifyFromAddress.Length - 1];

                                    if (!string.IsNullOrEmpty(FromAdd))
                                    {
                                        FromAdd = FromAdd.Replace("<", string.Empty);
                                        FromAdd = FromAdd.Replace(">", string.Empty);
                                    }
                                }
                            }
                            
                            MailBody = string.Empty;
                            if (MsgContent.Payload.Parts == null && MsgContent.Payload.Body != null)
                            {
                                MailBody = MsgContent.Payload.Body.Data;
                            }
                            else
                            {
                                MailBody = GmailAccess.MsgNestedParts(MsgContent.Payload.Parts);
                            }
                            
                            ReadableText = string.Empty;
                            ReadableText = GmailAccess.Base64Decode(MailBody);

                            if (!string.IsNullOrEmpty(ReadableText))
                            {
                                Gmail GMail = new Gmail();
                                GMail.From = FromAddress;
                                GMail.Body = ReadableText;
                                GMail.MailDateTime = Convert.ToDateTime(Date);
                                EmailList.Add(GMail);
                            }
                        }
                    }
                }
                return EmailList;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return null;
            }
        }

    private void ProcessMail(List<Gmail> mailList, Board[] currentBoards)
    {
        foreach (var mail in mailList)
        {
            Match match = mailRegex.Match(mail.Body);
            if (match.Success)
            {
                var s = match.Value;
                var data = s.Split('_');
                string name = data[0];
                string bName = data[1];
                int requestedNum = int.Parse(data[2]);

                foreach (var board in currentBoards)
                {
                    if (string.Equals(board.name, bName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (requestedNum <= board.numAvailable)
                        {
                            var row = Array.IndexOf(currentBoards, board) + 2;
                            var newNum = board.numAvailable - requestedNum;
                            
                            SheetAccess.WriteCell($"M{row}", newNum.ToString());
                            SheetAccess.WriteOwner($"A1", name, requestedNum.ToString());
                            board.numAvailable = newNum;
                        }
                        else
                        {
                            var service = GmailAccess.GetService();
                            
                            string message = $"To: {mail.From}\r\nSubject: Отсутствие платы\r\n" +
                                             $"Content-Type: text/html;charset=utf-8\r\n\r\n<h1>Запрашеваемая" +
                                             $" вами плата отсутствует, либо их недостаточно</h1>";
                            var msg = new Message();
                            msg.Raw = GmailAccess.Base64UrlEncode(message.ToString());

                            service.Users.Messages.Send(msg, mail.From);
                        }
                        
                        break;
                    }
                }
            }
        }
    }
}
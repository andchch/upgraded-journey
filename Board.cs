using System;

namespace kursovaya;

public class Board
{
    private string ChipName;
    private readonly int hasDDR;
    private readonly int hasEth;
    private readonly int hasHPS;
    private readonly int hasVGA;

    public string name;
    public int numAvailable;
    private readonly int numOfADCChannels;
    private readonly int numOfButtons;
    private readonly int numOfGPIO;
    private readonly int numOfLED;
    private readonly int numOfSwitches;
    private readonly int voltage;

    public float[] weights =
    {
        1, // hasHPS (true/false)  --> can be 0 or 1
        1, // numOfADCChannels     --> can from 0 up to 1
        1, // voltage              --> can from 0 up to 1
        1, // hasDDR (true/false)  --> can be 0 or 1
        1, // numOfButtons         --> can from 0 up to 1
        1, // numOfSwitches        --> can from 0 up to 1
        1, // numOfLED             --> can from 0 up to 1
        1, // numOfGPIO            --> can from 0 up to 1
        1, // hasVGA (true/false)  --> can be 0 or 1
        1 // hasEth (true/false)  --> can be 0 or 1
        //1  // chipName (equal/not) --> can be 0 or 1
    };

    public Board(string name, int hasHps, int numOfAdcChannels, int voltage, int hasDdr, int numOfButtons,
        int numOfSwitches, int numOfLed, int numOfGpio, int hasVga, int hasEth, string chipName, int numAvailable)
    {
        this.name = name;
        this.numAvailable = numAvailable;
        hasHPS = hasHps;
        numOfADCChannels = numOfAdcChannels;
        this.voltage = voltage;
        hasDDR = hasDdr;
        this.numOfButtons = numOfButtons;
        this.numOfSwitches = numOfSwitches;
        numOfLED = numOfLed;
        numOfGPIO = numOfGpio;
        hasVGA = hasVga;
        this.hasEth = hasEth;
        ChipName = chipName;
    }

    public static bool CheckAvaible(Board board)
    {
        return board.numAvailable != 0;
    }

    public float CalculateAbsoluteSimilarity(Board board, AppSettings appSettings)
    {
        float[] specs =
        {
            board.hasHPS,
            board.numOfADCChannels,
            board.voltage,
            board.hasDDR,
            board.numOfButtons,
            board.numOfSwitches,
            board.numOfLED,
            board.numOfGPIO,
            board.hasVGA,
            board.hasEth
        };

        return appSettings.weights[0] * specs[0] + appSettings.weights[1] * specs[1] +
               appSettings.weights[2] * specs[2] + appSettings.weights[3] * specs[3] +
               appSettings.weights[4] * specs[4] + appSettings.weights[5] * specs[5] +
               appSettings.weights[6] * specs[6] + appSettings.weights[7] * specs[7] +
               appSettings.weights[8] * specs[8] + appSettings.weights[9] * specs[9];
    }

    private static float CalculateRelativeSimilarity(Board currentBoard, Board targetBoard, AppSettings appSettings)
    {
        float[] currentBoardSpecs =
        {
            currentBoard.hasHPS,
            currentBoard.numOfADCChannels,
            currentBoard.voltage,
            currentBoard.hasDDR,
            currentBoard.numOfButtons,
            currentBoard.numOfSwitches,
            currentBoard.numOfLED,
            currentBoard.numOfGPIO,
            currentBoard.hasVGA,
            currentBoard.hasEth
        };
        float[] targetBoardSpecs =
        {
            targetBoard.hasHPS, // hasHPS
            targetBoard.numOfADCChannels, // numOfADCChannels
            targetBoard.voltage, // voltage
            targetBoard.hasDDR, // hasDDR
            targetBoard.numOfButtons, // numOfButtons
            targetBoard.numOfSwitches, // numOfSwitches
            targetBoard.numOfLED, // numOfLED
            targetBoard.numOfGPIO, // numOfGPIO
            targetBoard.hasVGA, // hasVGA
            targetBoard.hasEth // hasEth
        };
        var currentBoardSimilarity = appSettings.weights[0] * currentBoardSpecs[0] +
                                     appSettings.weights[1] * currentBoardSpecs[1] +
                                     appSettings.weights[2] * currentBoardSpecs[2] +
                                     appSettings.weights[3] * currentBoardSpecs[3] +
                                     appSettings.weights[4] * currentBoardSpecs[4] +
                                     appSettings.weights[5] * currentBoardSpecs[5] +
                                     appSettings.weights[6] * currentBoardSpecs[6] +
                                     appSettings.weights[7] * currentBoardSpecs[7] +
                                     appSettings.weights[8] * currentBoardSpecs[8] +
                                     appSettings.weights[9] * currentBoardSpecs[9];

        var targetBoardSimilarity = appSettings.weights[0] * targetBoardSpecs[0] +
                                    appSettings.weights[1] * targetBoardSpecs[1] +
                                    appSettings.weights[2] * targetBoardSpecs[2] +
                                    appSettings.weights[3] * targetBoardSpecs[3] +
                                    appSettings.weights[4] * targetBoardSpecs[4] +
                                    appSettings.weights[5] * targetBoardSpecs[5] +
                                    appSettings.weights[6] * targetBoardSpecs[6] +
                                    appSettings.weights[7] * targetBoardSpecs[7] +
                                    appSettings.weights[8] * targetBoardSpecs[8] +
                                    appSettings.weights[9] * targetBoardSpecs[9];

        return Math.Abs(targetBoardSimilarity - currentBoardSimilarity);
    }

    public static Board SuggestEqual(Board[] boards, Board targetBoard, AppSettings appSettings)
    {
        var index = 0;

        float currentSimilarity;
        float maxSimilarity = 0;
        for (var i = 0; i < boards.Length; i++)
        {
            currentSimilarity = CalculateRelativeSimilarity(boards[i], targetBoard, appSettings);

            if (maxSimilarity < currentSimilarity && boards[i].numAvailable != 0)
            {
                maxSimilarity = currentSimilarity;
                index = i;
            }
        }

        return boards[index];
    }
}
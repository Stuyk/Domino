using Domino.Data;
using GTANetworkAPI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Domino
{
    public class Main : Script
    {
        /// <summary>
        /// Console Output String
        /// </summary>
        public static string Domino = "Domino ->";
        /// <summary>
        /// Storage Directory
        /// </summary>
        public static string directoryPath = "bridge/resources/Domino/Transactions";
        /// <summary>
        /// File Directory + Name
        /// </summary>
        public static string filePath = $"{directoryPath}/transactions.txt";
        /// <summary>
        /// The maximum transactions allowed before a new block is generated.
        /// </summary>
        public static int MaxTransactionsPerBlock = 25;
        /// <summary>
        /// Block Reward
        /// </summary>
        public static int BlockReward = 1;
        /// <summary>
        /// Max Printed Cash Available
        /// </summary>
        public static int MaxCoinCount = 25;

        public Main()
        {
            Event.OnResourceStart += StartResource;
        }

        /// <summary>
        /// Checks if the directory / file exists. If not it creates it.
        /// </summary>
        private void StartResource()
        {
            API.ConsoleOutput($"{Domino} Starting resource...");

            Words.LoadAllWords();
            API.ConsoleOutput($"{Domino} Word Count: {Words.List.Length}");

            if (!Directory.Exists(directoryPath))
            {
                API.ConsoleOutput($"{Domino} Creating directory...");
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(filePath))
            {
                API.ConsoleOutput($"{Domino} Creating genesis block...");
                generateGenesisBlock();
            }

            DataHandler.LoadAllBlocks();
        }

        /// <summary>
        /// Generates the genesis block for the block chain.
        /// </summary>
        private void generateGenesisBlock()
        {
            API.ConsoleOutput($"{Domino} Creating genesis block...");

            Block genesisBlock = new Block();
            genesisBlock.PreviousHash = EasyEncryption.SHA.ComputeSHA256Hash("Domino");
            genesisBlock.BlockHash = EasyEncryption.SHA.ComputeSHA256Hash("BlockChain");
            genesisBlock.Transactions = new List<Transaction>();

            Transaction newTransaction = new Transaction()
            {
                Value = 0,
                FromAddress = EasyEncryption.SHA.ComputeSHA256Hash("Created By"),
                TargetAddress = EasyEncryption.SHA.ComputeSHA256Hash("Stuyk")
            };

            genesisBlock.Transactions.Add(newTransaction);

            DataHandler.WriteBlock(genesisBlock);

            API.ConsoleOutput($"{Domino} Genesis block created.");
        }
    }
}

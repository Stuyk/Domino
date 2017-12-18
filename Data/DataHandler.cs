using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace Domino.Data
{
    public static class DataHandler
    {
        /// <summary>
        /// Where we keep a Queue of all the blocks that need to be pushed to the Database.
        /// </summary>
        public static Queue<Block> Queue = new Queue<Block>();

        /// <summary>
        /// An array of all the blocks available.
        /// </summary>
        public static Block[] Blocks { get; set; }

        /// <summary>
        /// The CurrentBlock we're writing transactions to.
        /// </summary>
        public static Block CurrentBlock { get; set; }

        /// <summary>
        /// Used to control if a file is done writing.
        /// </summary>
        public static bool FileDoneWriting = true;

        /// <summary>
        /// Create a new transaction to add to the newest block.
        /// </summary>
        /// <param name="transaction"></param>
        public static void CreateNewTransaction(Transaction transaction)
        {
            if (CurrentBlock == null)
            {
                CurrentBlock = new Block();
                CurrentBlock.Transactions = new List<Transaction>();
            }

            if (CurrentBlock.Transactions.Count >= Main.MaxTransactionsPerBlock)
            {
                if (CurrentBlock != null)
                    Queue.Enqueue(CurrentBlock);

                CurrentBlock = new Block();
                CurrentBlock.Transactions = new List<Transaction>();
            }

            CurrentBlock.Transactions.Add(transaction);
        }

        /// <summary>
        /// Get the player's public address for transactions.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static string GetPlayerPublicAddress(Client player)
        {
            // Check if they have a secret.
            if (!player.HasData("Domino_Public"))
                return null;

            // Reward the player.
            string publicHash = player.GetData("Domino_Public");
            return publicHash;
        }

        /// <summary>
        /// Mine the Current Block Available.
        /// </summary>
        public static void MineCurrentBlock(string rewardAddress)
        {
            if (Queue.Count > 0)
            {
                if (!FileDoneWriting)
                    return;

                FileDoneWriting = false;

                Block targetBlock = Queue.Dequeue();

                if (GetCirculatingSupply() < Main.MaxCoinCount)
                {
                    targetBlock.Transactions.Add(new Transaction()
                    {
                        TargetAddress = rewardAddress,
                        FromAddress = EasyEncryption.SHA.ComputeSHA256Hash("Reward"),
                        Value = Main.BlockReward
                    });
                }

                WriteBlock(targetBlock);

                Console.WriteLine($"Current Queue Left: {DataHandler.Queue.Count}");
            }
        }

        /// <summary>
        /// Write our Block + Transactions to the file.
        /// </summary>
        /// <param name="block"></param>
        public static void WriteBlock(Block block)
        {
            FileDoneWriting = false;

            if (Blocks != null)
            {
                block.PreviousHash = Blocks[Blocks.Length - 1].BlockHash;
                block.BlockHash = EasyEncryption.SHA.ComputeSHA256Hash(JsonConvert.SerializeObject(block.Transactions) + Blocks[Blocks.Length - 1].BlockHash + block.PreviousHash);
            }

            string newBlock = JsonConvert.SerializeObject(block);

            if (Blocks != null)
            {
                File.AppendAllText(Main.filePath, Environment.NewLine + newBlock);
            }
            else
            {
                File.AppendAllText(Main.filePath, newBlock);
            }

            LoadAllBlocks();
        }

        /// <summary>
        /// Load all the current blocks in the Block Chain.
        /// </summary>
        public static void LoadAllBlocks()
        {
            API.Shared.ConsoleOutput($"{Main.Domino} Loading all blocks...");

            string[] lines = File.ReadAllLines(Main.filePath);
            Block[] blocks = new Block[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                blocks[i] = DeserializeBlock.FromJson(lines[i]);
            }

            Blocks = blocks;

            FileDoneWriting = true;
            API.Shared.ConsoleOutput($"{Main.Domino} Blocks done loading.");

            Verification.VerifyAllPreviousBlocks();
        }

        /// <summary>
        /// Get an address's sent transactions.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static long GetAllSentTransactions(string address)
        {
            long recievedTotal = 0;

            foreach (Block block in Blocks)
            {
                foreach (Transaction transaction in block.Transactions)
                {
                    if (transaction.FromAddress == address)
                    {
                        recievedTotal += transaction.Value;
                    }
                }
            }
            return recievedTotal;
        }

        /// <summary>
        /// Get an addresse's recieved transactions.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static long GetAllRecievedTransactions(string address)
        {
            long recievedTotal = 0;

            foreach (Block block in Blocks)
                foreach (Transaction transaction in block.Transactions)
                    if (transaction.TargetAddress == address)
                        recievedTotal += transaction.Value;

            return recievedTotal;
        }

        /// <summary>
        /// Get a player's balance based on their address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static int GetPlayerBalance(string address)
        {
            return Convert.ToInt32(GetAllRecievedTransactions(address) - GetAllSentTransactions(address));
        }

        /// <summary>
        /// Get the Circulating Supply in the economy currently.
        /// </summary>
        /// <returns></returns>
        public static int GetCirculatingSupply()
        {
            long recievedTotal = 0;

            foreach (Block block in Blocks)
            {
                foreach (Transaction transaction in block.Transactions)
                {
                    recievedTotal += transaction.Value;
                }
            }

            return Convert.ToInt32(recievedTotal);
        }
    }
}

using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domino.Data
{
    public class Commands : Script
    {
        [Command("MarketCap")]
        public void cmdMarketCap(Client player)
        {
            int circulation = DataHandler.GetCirculatingSupply();
            API.SendChatMessageToPlayer(player, $"{circulation}/{Main.MaxCoinCount}");
        }


        [Command("Interact")]
        public void cmdInteraction(Client player)
        {
            API.SendChatMessageToPlayer(player, "You did something, let's mine a block because of it...");

            string publicHash = DataHandler.GetPlayerPublicAddress(player);

            DataHandler.MineCurrentBlock(publicHash);
        }

        [Command("Balance")]
        public void cmdBalance(Client player)
        {
            // Check if they have a secret.
            if (!player.HasData("Domino_Public"))
                return;

            // Reward the player.
            string publicHash = player.GetData("Domino_Public");

            player.SendChatMessage($"Player Balance: {DataHandler.GetPlayerBalance(publicHash)}");
        }

        [Command("SetSecret", GreedyArg = true)]
        public void cmdSecret(Client player, string secretCode)
        {
            if (secretCode == null)
                return;
            // Encrypt Player Name
            string playerHash = EasyEncryption.SHA.ComputeSHA256Hash(player.Name.ToLower());
            // Encrypt Secret String into Private Hash
            string privateHash = EasyEncryption.SHA.ComputeSHA256Hash(secretCode.ToLower());
            // Encrypt Secret String + Private Hash for Public Address
            string publicHash = EasyEncryption.SHA.ComputeSHA256Hash(playerHash + privateHash);
            // Store Public Hash on Player
            player.SetData("Domino_Public", publicHash);
            player.SendChatMessage("Secret Added.");
        }

        [Command("VerifySecret", GreedyArg = true)]
        public void cmdVerifySecret(Client player, string secretCode)
        {
            // Encrypt Player Name
            string playerHash = EasyEncryption.SHA.ComputeSHA256Hash(player.Name.ToLower());
            // Encrypt Secret String into Private Hash
            string privateHash = EasyEncryption.SHA.ComputeSHA256Hash(secretCode.ToLower());
            // Encrypt Secret String + Private Hash for Public Address
            string publicHash = EasyEncryption.SHA.ComputeSHA256Hash(playerHash + privateHash);

            if (publicHash == player.GetData("Domino_Public"))
            {
                player.SendChatMessage("Successfully Verified Secret.");
            }
            else
            {
                player.SendChatMessage("Secret did not match.");
            }
        }

        [Command("genBlocks", GreedyArg = true)]
        public void cmdGenBlocks(Client player)
        {
            for (int i = 0; i < 100; i++)
            {
                DataHandler.CreateNewTransaction(new Transaction()
                {
                    FromAddress = EasyEncryption.SHA.ComputeSHA256Hash(WordList.GetWordRepeatable()),
                    TargetAddress = EasyEncryption.SHA.ComputeSHA256Hash(WordList.GetWordRepeatable()),
                    Value = 0
                });
            }
        }
    }
}

using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domino.Data
{
    public static class Verification
    {
        public static bool VerifyAllPreviousBlocks()
        {
            API.Shared.ConsoleOutput($"{Main.Domino} Verifying all previous blocks...");
            string previousHash = null;

            for (int i = 0; i < DataHandler.Blocks.Length; i++)
            {
                if (previousHash == null)
                {
                    previousHash = DataHandler.Blocks[0].PreviousHash;
                }

                if (DataHandler.Blocks[i].PreviousHash != previousHash)
                {
                    API.Shared.ConsoleOutput($"{Main.Domino} The database failed to verify, stopping server in 5 seconds.");
                    var task = Task.Run(() =>
                    {
                        /// insert 5 second stop
                    });
                    return false;
                }

                previousHash = DataHandler.Blocks[i].BlockHash;
            }

            API.Shared.ConsoleOutput($"{Main.Domino} The database has verified successfully.");
            return true;
        }
    }
}

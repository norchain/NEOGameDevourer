using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using Helper = Neo.SmartContract.Framework.Helper;
using System.Text;

using System.ComponentModel;
using System.Numerics;
using System.Collections.Generic;


namespace SC
{


    public class Class : SmartContract
    {
        [Serializable]
        public class Card
        {
            public byte[] id;
            public BigInteger lvl;
            public byte[] ownerId;
        }

        private static readonly byte[] GAS_ID = { 231, 45, 40, 105, 121, 238, 108, 177, 183, 230, 93, 253, 223, 178, 227, 132, 16, 11, 141, 20, 142, 119, 88, 222, 66, 228, 22, 139, 113, 121, 44, 96 };


        public static byte[] GetTransReceiver()
        {
            return ExecutionEngine.ExecutingScriptHash;
        }

        public static byte[] GetTransSender()
        {
            Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
            TransactionInput[] inputs = tx.GetInputs();
            if (inputs.Length > 0)
                return inputs[0].p;
            return null;
        }

        public static BigInteger GetGASAttached()
        {
            Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
            foreach (TransactionOutput output in tx.GetOutputs())
            {
                if (output.ScriptHash == ExecutionEngine.ExecutingScriptHash
                    && output.AssetId == GAS_ID)
                    return output.Value;
            }
            return 0;
        }

        public static byte[] GetTxHashForRandomness()
        {
            //[Modify] Randomness by the txid as we discussed yesterday (Blockchain.GetTransaction())
            Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
            return tx.Hash;
        }

        public static readonly BigInteger TokenPerGas = 100;
        public static readonly byte[] Owner = "AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y".ToScriptHash();

        public static object Main(string op, params object[] args)
        {
            if (Runtime.Trigger == TriggerType.Verification)
            {
                if (op == "buyToken")
                {
                    byte[] receiver = GetTransReceiver();
                    if (receiver == Owner)
                    {
                        return BuyToken();
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            if (Runtime.Trigger == TriggerType.Application)
            {

                if (op == "buyCard")   //Purchase a card with a level
                {
                    BigInteger level = (BigInteger)args[1];
                    return buyCard(level);
                }


                return false;
            }
            else
            {
                return false;
            }
        }



        public static byte[] buyCard(BigInteger level)
        {
            BigInteger price = 10 * level;
            byte[] acc = GetTransSender();
            BigInteger nowTokens = GetToken(acc);
            if (nowTokens >= price)
            {
                SetToken(acc, nowTokens - price);
                Card card = new Card
                {
                    id = GetTxHashForRandomness(),
                    lvl = level,
                    ownerId = acc
                };
                Storage.Put(Storage.CurrentContext, "c" + card.id, card.Serialize());
                return card.id;
            }
            else
            {
                return new byte[0];
            }

        }

        public static bool BuyToken()
        {
            byte[] account = GetTransSender();
            BigInteger numGASAttached = GetGASAttached();
            BigInteger numToken = numGASAttached * TokenPerGas;
            BigInteger origToken = GetToken(account);

            Storage.Put(Storage.CurrentContext, "t" + account.AsString(), (origToken + numToken).AsByteArray().AsString());
            return true;
        }

        public static BigInteger GetToken(byte[] account)
        {
            return Storage.Get(Storage.CurrentContext, "t" + account.AsString()).AsBigInteger();
        }

        public static void SetToken(byte[] account, BigInteger amount)
        {
            Storage.Put(Storage.CurrentContext, "t" + account.AsString(), amount.AsByteArray().AsString());
        }

        public static Card GetCard(byte[] id)
        {
            return (Card)Storage.Get(Storage.CurrentContext, "c" + id).Deserialize();
        }
    }
}

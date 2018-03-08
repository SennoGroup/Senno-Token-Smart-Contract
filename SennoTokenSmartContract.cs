    public class Contract : SmartContract
    {
        public static object Main(string method, params object[] args)
        {
            string name = "Senno";
            string symbol = "SEN";
            BigInteger decimals = 0;         	
	
            var sencrptbts = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            if (method == "deploy") return Deploy(sencrptbts);
            if (method == "totalSupply") return Storage.Get(Storage.CurrentContext, "supply");
            if (method == "name") return name;
            if (method == "symbol") return symbol;
            if (method == "decimals") return decimals;
            if (method == "balanceOf") return Storage.Get(Storage.CurrentContext, (byte[])args[0]);
            //Verify that the originator is honest.
            if (!Runtime.CheckWitness((byte[])args[0])) return false;
            if (method == "transfer") return Transfer((byte[])args[0], (byte[])args[1], BytesToInt((byte[])args[2]));

            return false;
        }

        private static bool Deploy(byte[] sencrptbts)
        {
            BigInteger initSupply = 10000000000;
            Storage.Put(Storage.CurrentContext, sencrptbts, IntToBytes(initSupply));
            Storage.Put(Storage.CurrentContext, "supply", IntToBytes(initSupply));
            return true;
        }

        private static bool Transfer(byte[] originator, byte[] to, BigInteger amount)
        {
            //Get the account value of the source and destination accounts.
            var originatorValue = Storage.Get(Storage.CurrentContext, originator);
            var targetValue = Storage.Get(Storage.CurrentContext, to);

            BigInteger nOriginatorValue = BytesToInt(originatorValue) - amount;
            BigInteger nTargetValue = BytesToInt(targetValue) + amount;

            //If the transaction is valid, proceed.
            if (nOriginatorValue >= 0 && amount >= 0)
            {
                Storage.Put(Storage.CurrentContext, originator, IntToBytes(nOriginatorValue));
                Storage.Put(Storage.CurrentContext, to, IntToBytes(nTargetValue));
                Runtime.Notify("Transfer Successful", originator, to, amount, Blockchain.GetHeight());
                return true;
            }
            return false;
        }

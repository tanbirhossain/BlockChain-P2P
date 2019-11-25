using System;
using System.Collections.Generic;
using System.Text;

namespace BlockchainDemo
{
    public class Blockchain
    {
        public IList<Transaction> PendingTransactions = new List<Transaction>();
        public IList<Block> Chain { set;  get; }
        public int Difficulty { set; get; } = 2;
        public int Reward = 1; //1 cryptocurrency

        public Blockchain()
        {
            
        }


        public void InitializeChain()
        {
            Chain = new List<Block>();
            AddGenesisBlock();
        }

        public Block CreateGenesisBlock()
        {
            Block block = new Block(DateTime.Now, null, PendingTransactions);

            block.Mine(Difficulty);
            PendingTransactions = new List<Transaction>();
            return block;
        }

        public void AddGenesisBlock()
        {
            Chain.Add(CreateGenesisBlock());
        }
        
        public Block GetLatestBlock()
        {
            return Chain[Chain.Count - 1];
        }
        /// <summary>
        /// 1. CreateTransaction . simple add a transaction into transaction list
        /// </summary>
        /// <param name="transaction"></param>
        public void CreateTransaction(Transaction transaction)
        {
            PendingTransactions.Add(transaction); 
        }
        /// <summary>
        /// 2. 2nd minar part. this time transaction will finalized
        /// </summary>
        /// <param name="minerAddress"></param>
        public void ProcessPendingTransactions(string minerAddress)
        {
            //we have to keep in mind every time mining means  one block complete .
            //all pending transaction and previous transaction hash linked in this block
            Block block = new Block(DateTime.Now, GetLatestBlock().Hash, PendingTransactions);//B-1

            //here is populate block other variables and this block to the Block list chain
            AddBlock(block);

            // already we have created block so now we will create new tranactionlist, 
            // now pending transationlist fully empty
            PendingTransactions = new List<Transaction>(); 
            CreateTransaction(new Transaction(null, minerAddress, Reward));
        }

        public void AddBlock(Block block) //part of - ProcessPendingTransactions
        {
            Block latestBlock = GetLatestBlock();
            block.Index = latestBlock.Index + 1;//3
            block.PreviousHash = latestBlock.Hash;//4 -- this is no need to push here bcoz we already populate this hash
            //block.Hash = block.CalculateHash();
            block.Mine(this.Difficulty);//5 - this takes so many much time for making the  Difficulty label hash


           //whole block chain here. this chain list store all completed trasaction block
            Chain.Add(block);
        }

        public bool IsValid()
        {
            for (int i = 1; i < Chain.Count; i++)
            {
                Block currentBlock = Chain[i];
                Block previousBlock = Chain[i - 1];

                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    return false;
                }
                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }
            }
            return true;
        }

        public int GetBalance(string address)
        {
            int balance = 0;

            for (int i = 0; i < Chain.Count; i++)
            {
                for (int j = 0; j < Chain[i].Transactions.Count; j++)
                {
                    var transaction = Chain[i].Transactions[j];

                    if (transaction.FromAddress == address)
                    {
                        balance -= transaction.Amount;
                    }

                    if (transaction.ToAddress == address)
                    {
                        balance += transaction.Amount;
                    }
                }
            }

            return balance;
        }
    }
}

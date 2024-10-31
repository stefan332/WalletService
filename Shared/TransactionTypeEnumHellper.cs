using Data.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class TransactionTypeEnumHellper
    {
        public static TransactionTypeEnum GetTransactionTypeById(int id)
        {
            return id switch
            {
                1 => TransactionTypeEnum.Deposit,
                2 => TransactionTypeEnum.Withdrawal,
                3 => TransactionTypeEnum.BetPlacement,
                4 => TransactionTypeEnum.WinPayout,
                5 => TransactionTypeEnum.Bonus,
                _ => throw new ArgumentOutOfRangeException(nameof(id), "Invalid transaction type ID.")
            };
        }

        private static string GetEnumDesrctiption(TransactionTypeEnum typeEnum)
        {
            switch (typeEnum)
            {
                case TransactionTypeEnum.Deposit:
                    return "Deposit Funds/Adding funds to the wallet";
                case TransactionTypeEnum.Withdrawal:
                    return "Removing funds from the wallet";
                case TransactionTypeEnum.BetPlacement:
                    return "Placing a bet, which decreases the wallet balance";
                case TransactionTypeEnum.WinPayout:
                    return "Winning a bet, which increases the wallet balance";
                case TransactionTypeEnum.Bonus:
                    return "Bonuses added to the wallet";
                default:
                    return typeEnum.ToString();
            }
        }

        public static string MapDescrition(TransactionTypeEnum typeEnum)
        {
            return GetEnumDesrctiption(typeEnum);
        }
    }

    public enum TransactionTypeEnum
    {
        [Description("Deposit Funds/Adding funds to the wallet")]
        Deposit = 1,       // Adding funds to the wallet
        [Description("Removing funds from the wallet")]
        Withdrawal = 2,    // Removing funds from the wallet
        [Description("Placing a bet, which decreases the wallet balance")]
        BetPlacement = 3,  // Placing a bet, which decreases the wallet balance
        [Description("Winning a bet, which increases the wallet balance")]
        WinPayout = 4,     // Winning a bet, which increases the wallet balance
        [Description("Bonuses added to the wallet")]
        Bonus = 5           // Bonuses added to the wallet
    }
}

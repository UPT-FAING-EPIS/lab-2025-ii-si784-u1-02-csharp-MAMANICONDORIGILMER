using Bank.Domain.Models;
using NUnit.Framework;

namespace Bank.Domain.Tests
{
    public class BankAccountTests
    {
        [Test]
        public void Debit_WithValidAmount_UpdatesBalance()
        {
            // Arrange
            double beginningBalance = 11.99;
            double debitAmount = 4.55;
            double expected = 7.44;
            BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            // Act
            account.Debit(debitAmount);

            // Assert
            Assert.That(account.Balance, Is.EqualTo(expected).Within(0.001), 
                "Account not debited correctly");
        }

        [Test]
        public void Credit_WithValidAmount_UpdatesBalance()
        {
            // Arrange
            double beginningBalance = 11.99;
            double creditAmount = 5.01;
            double expected = 17.00;
            BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            // Act
            account.Credit(creditAmount);

            // Assert
            Assert.That(account.Balance, Is.EqualTo(expected).Within(0.001),
                "Account not credited correctly");
        }

        [Test]
        public void Debit_WithAmountGreaterThanBalance_Throws()
        {
            // Arrange
            double beginningBalance = 11.99;
            double debitAmount = 20.00;
            var account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => account.Debit(debitAmount));
        }

        [Test]
        public void Debit_WithNegativeAmount_Throws()
        {
            // Arrange
            double beginningBalance = 11.99;
            double debitAmount = -1.00;
            var account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => account.Debit(debitAmount));
        }

        [Test]
        public void Credit_WithNegativeAmount_Throws()
        {
            // Arrange
            double beginningBalance = 11.99;
            double creditAmount = -5.00;
            var account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => account.Credit(creditAmount));
        }
    }
}
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Beis.HelpToGrow.Voucher.Api.Check.Common;
using Beis.HelpToGrow.Voucher.Api.Check.Common;
using Beis.HelpToGrow.Voucher.Api.Check.Controllers;
using Beis.HelpToGrow.Voucher.Api.Check.Domain.Entities;
using Beis.HelpToGrow.Voucher.Api.Check.Interfaces;

namespace Beis.HelpToGrow.Voucher.Api.Check.Tests
{
    [TestFixture]
    public class AesEncryptionTests
    {
        private IEncryptionService _encryptionService;
        private string _salt;
        private int _passwordIterations;
        private string _initialVector;
        private int _keySize;
        private string PlainText = "plainText";
        private string Password = "password";
        private string EncryptedString = "ujd9uSDwqMTyQ8IzeEJtGw==";

        [SetUp]
        public void Setup()
        {
            _salt = "Kosherista";
            _passwordIterations = 2;
            _initialVector = "OFRna73m*aze01xY";
            _keySize = 256;
            IOptions<EncryptionSettings> _options = Options.Create<EncryptionSettings>(new EncryptionSettings 
                { 
                    VOUCHER_ENCRYPTION_SALT = _salt, 
                    VOUCHER_ENCRYPTION_ITERATION = _passwordIterations, 
                    VOUCHER_ENCRYPTION_INITIAL_VECTOR = _initialVector, 
                    VOUCHER_ENCRYPTION_KEY_SIZE = _keySize 
                });
            _encryptionService = new AesEncryption(_options);
        }
        
        [Test]
        public void CallingEncryptReturnsValidEncodedString()
        {
            SetUpDefaultValues();
            var response = _encryptionService.Encrypt(PlainText, Password);
            Assert.AreEqual(EncryptedString, response);
        }

        [Test]
        public void CallingEncryptWithEmptyPlainTextReturnsEmptyString()
        {
            SetUpDefaultValues();
            PlainText = "";
            var response = _encryptionService.Encrypt(PlainText, Password);
            Assert.AreEqual(string.Empty, response);
        }

        [Test]
        public void CallingEncryptWithNullPlainTextReturnsEmptyString()
        {
            SetUpDefaultValues();
            PlainText = null;
            var response = _encryptionService.Encrypt(PlainText, Password);
            Assert.AreEqual(string.Empty, response);
        }

        [Test]
        public void CallingDecryptReturnsValidPlainTextString()
        {
            SetUpDefaultValues();
            var response = _encryptionService.Decrypt(EncryptedString, Password);
            Assert.AreEqual(PlainText, response);
        }

        [Test]
        public void CallingDecryptWithEmptyEncryptedStringReturnsEmptyString()
        {
            SetUpDefaultValues();
            EncryptedString = "";
            var response = _encryptionService.Decrypt(EncryptedString, Password);
            Assert.AreEqual(string.Empty, response);
        }

        [Test]
        public void CallingDecryptWithNullEncryptedStringReturnsEmptyString()
        {
            SetUpDefaultValues();
            EncryptedString = null;
            var response = _encryptionService.Decrypt(EncryptedString, Password);
            Assert.AreEqual(string.Empty, response);
        }

        private void SetUpDefaultValues()
        {
            PlainText = "plainText";
            Password = "password";
            EncryptedString = "ujd9uSDwqMTyQ8IzeEJtGw==";
        }
    }
}
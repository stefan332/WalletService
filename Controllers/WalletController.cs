using AuthorizaionPolcies;
using BAL.Contratcs;
using BAL.Contratcs.Transactions;
using BAL.Models.Requests;
using BAL.Models.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace WalletService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(PlayerOnlyAccess), AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WalletController : ControllerBase
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<WalletController> _logger;
        private readonly IWalletService _walletService;
        private readonly ITransactionService _transactionService;

        public WalletController(ILogger<WalletController> logger,
                                IWalletService walletService,
                                ITransactionService transactionService,
                                UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _walletService = walletService;
            _transactionService = transactionService;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("GetUserWallet")]
        public async Task<IActionResult> GetUserWallet(int walletId)
        {
            try
            {

                var wallet = await _walletService.GetWalletAsync(walletId);
                var walletDto = new WalletDto
                {
                    Id = wallet.Id,
                    Balance = wallet.Balance,
                    UserId = wallet.UserId,

                };
                return Ok(walletDto);

            }
            catch (Exception exc)
            {
                return NotFound(exc.Message);
            }

        }

        [HttpGet]
        [Route("GetUserBallance")]
        public async Task<IActionResult> GetUserBallance(string userId)
        {
            try
            {
                var msg = await CheckIfUserExists(userId);
                if (!string.IsNullOrEmpty(msg)) throw new NotFoundException(msg);

                var wallet = await _walletService.GetBalanceAsync(userId);
                return Ok(wallet);

            }
            catch (Exception exc)
            {
                return NotFound(exc.Message);
            }

        }

        [HttpPost]
        [Route("CreateWallet")]
        public async Task<IActionResult> CreateWallet(CreateWalletDto createWalletDto)
        {
            try
            {
                var msg = await CheckIfUserExists(createWalletDto.UserId);
                if (!string.IsNullOrEmpty(msg)) throw new NotFoundException(msg);
                var wallet = await _walletService.CreateWalletAsync(createWalletDto.UserId);

                return CreatedAtAction(nameof(GetUserWallet), new { walletId = wallet.Id, wallet });

            }
            catch (Exception exc)
            {
                return NotFound(exc.Message);
            }
        }


        [HttpPost]
        [Route("DepositFunds")]
        public async Task<IActionResult> DepositFunds(AddOrRemoveFundsRequest addOrRemoveFundsRequest)
        {
            try
            {
                var checkCorrecetTransactionType = TransactionTypeEnumHellper.GetTransactionTypeById(addOrRemoveFundsRequest.TransactionTypeId);
                if (checkCorrecetTransactionType != TransactionTypeEnum.Deposit)
                    throw new Exception(String.Format("Wrong Transaction Type|{0}", addOrRemoveFundsRequest.TransactionTypeId));
                var transaction = await _transactionService.AddFundsAsync(addOrRemoveFundsRequest);

                return CreatedAtAction(nameof(GetUserWallet), new { walletId = addOrRemoveFundsRequest.WalletID, transaction });

            }
            catch (Exception exc)
            {
                return NotFound(exc.Message);
            }
        }

        [HttpPost]
        [Route("WinPayout")]
        public async Task<IActionResult> WinPayout(AddOrRemoveFundsRequest addOrRemoveFundsRequest)
        {
            try
            {
                var checkCorrecetTransactionType = TransactionTypeEnumHellper.GetTransactionTypeById(addOrRemoveFundsRequest.TransactionTypeId);
                if (checkCorrecetTransactionType != TransactionTypeEnum.WinPayout)
                    throw new Exception(String.Format("Wrong Transaction Type|{0}", addOrRemoveFundsRequest.TransactionTypeId));
                var transaction = await _transactionService.AddFundsAsync(addOrRemoveFundsRequest);

                return CreatedAtAction(nameof(GetUserWallet), new { walletId = addOrRemoveFundsRequest.WalletID, transaction });

            }
            catch (Exception exc)
            {
                return NotFound(exc.Message);
            }
        }

        [HttpPost]
        [Route("Bonus")]
        public async Task<IActionResult> Bonus(AddOrRemoveFundsRequest addOrRemoveFundsRequest)
        {
            try
            {
                var checkCorrecetTransactionType = TransactionTypeEnumHellper.GetTransactionTypeById(addOrRemoveFundsRequest.TransactionTypeId);
                if (checkCorrecetTransactionType != TransactionTypeEnum.Bonus)
                    throw new Exception(String.Format("Wrong Transaction Type|{0}", addOrRemoveFundsRequest.TransactionTypeId));
                var transaction = await _transactionService.AddFundsAsync(addOrRemoveFundsRequest);

                return CreatedAtAction(nameof(GetUserWallet), new { walletId = addOrRemoveFundsRequest.WalletID, transaction });

            }
            catch (Exception exc)
            {
                return NotFound(exc.Message);
            }
        }

        [HttpPost]
        [Route("Withdrawal")]
        public async Task<IActionResult> Withdrawal(AddOrRemoveFundsRequest addOrRemoveFundsRequest)
        {
            try
            {
                var checkCorrecetTransactionType = TransactionTypeEnumHellper.GetTransactionTypeById(addOrRemoveFundsRequest.TransactionTypeId);
                if (checkCorrecetTransactionType != TransactionTypeEnum.Withdrawal)
                    throw new Exception(String.Format("Wrong Transaction Type|{0}", addOrRemoveFundsRequest.TransactionTypeId));
                var transaction = await _transactionService.RemoveFundsAsync(addOrRemoveFundsRequest);

                return CreatedAtAction(nameof(GetUserWallet), new { walletId = addOrRemoveFundsRequest.WalletID, transaction });

            }
            catch (Exception exc)
            {
                return NotFound(exc.Message);
            }
        }

        [HttpPost]
        [Route("BetPlacementWithdrawal")]
        public async Task<IActionResult> BetPlacementWithdrawal(AddOrRemoveFundsRequest addOrRemoveFundsRequest)
        {
            try
            {
                var checkCorrecetTransactionType = TransactionTypeEnumHellper.GetTransactionTypeById(addOrRemoveFundsRequest.TransactionTypeId);
                if (checkCorrecetTransactionType != TransactionTypeEnum.BetPlacement)
                    throw new Exception(String.Format("Wrong Transaction Type|{0}", addOrRemoveFundsRequest.TransactionTypeId));
                var transaction = await _transactionService.RemoveFundsAsync(addOrRemoveFundsRequest);

                return CreatedAtAction(nameof(GetUserWallet), new { walletId = addOrRemoveFundsRequest.WalletID, transaction });

            }
            catch (Exception exc)
            {
                return NotFound(exc.Message);
            }
        }
        private async Task<string> CheckIfUserExists(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return "user does not exists";
            return string.Empty;

        }
    }
}
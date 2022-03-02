using BLL.DTO;
using BLL.DTO.VM;
using BLL.Interfaces;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prod.Filters;
using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace prod.Controllers
{
    [ServiceFilter(typeof(ExceptionFilter))]
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ICacheService _cacheService;

        public UserController(IUserService userService, ITokenService tokenService, ICacheService cacheService)
        {
            _userService = userService;
            _tokenService = tokenService;
            _cacheService = cacheService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Test")]
        public async Task<IActionResult> Test()
        {
            var user = await _cacheService.GetUserByIdAsync(1);

            return Ok(user.Email);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Registration")]
        public async Task<IActionResult> AddUser(VMAddUser vMAddUser)
        {
            try
            {
                UserDTO userDTO = AutoMapperService<VMAddUser, UserDTO>.Mapper(vMAddUser);
                await _userService.AddUser(userDTO);
                return Ok("AddUser - Success");
            }
            catch (ValidationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IEnumerable<int> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5)
            .ToArray();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> LoginUser(VMLoginUser vMLoginUser)
        {
            try
            {
                var user = await _cacheService.GetUserByIdAsync(1);

                UserDTO resultUserDTO = await _userService.LoginUser(new UserDTO
                {
                    Email = vMLoginUser.UserName,
                    Password = vMLoginUser.Password
                });

                var tokens = await _tokenService.CreateToken(resultUserDTO.Id);

                return Ok(tokens);
            }
            catch (ValidationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Refresh")]
        public async Task<IActionResult> Refresh(LoginResponseVM loginResponseVM)
        {
            var tokens = await _tokenService.RefreshTokenAsync(loginResponseVM.Token, loginResponseVM.RefreshToken);

            return Ok(tokens);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Donate")]
        public async Task<IActionResult> Processing(string token, string stripeEmail, int value)
        {
            try
            {
                // PaymentIntent payment = new PaymentIntent();
                /*   payment.SourceId = token;
                   payment.ReceiptEmail = stripeEmail;*/
                var serviceMeth = new PaymentMethodService();
                var paymentMethod = serviceMeth.Get(token);
                var optionsIntent = new PaymentIntentCreateOptions
                {
                    PaymentMethod = paymentMethod.Id,
                    Amount = value,
                    Description = "Test",
                    Currency = "usd",
                    ReceiptEmail = stripeEmail,
                };
                var serviceIntent = new PaymentIntentService();
                var paymentIntent = serviceIntent.Create(optionsIntent);
                var optionsConfirm = new PaymentIntentConfirmOptions
                {
                    PaymentMethod = paymentMethod.Id
                };
                serviceIntent.Confirm(paymentIntent.Id, optionsConfirm);
  

                return Ok("smth");
               /* var optionsToken = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Number = vMCard.CardNumber,
                        ExpMonth = vMCard.Month,
                        ExpYear = vMCard.Year,
                        Cvc = vMCard.Cvc,
                    }
                };

                var serviceToken = new TokenService();
                Token stripeToken = await serviceToken.CreateAsync(optionsToken);

                var chargeOptions = new ChargeCreateOptions
                {
                    Amount = vMCard.Value,
                    Currency = "usd",
                    Description = "test",
                    Source = stripeToken.Id
                };

                var service = new ChargeService();
                Charge charge = await service.CreateAsync(chargeOptions);

                if (charge.Paid)
                {
                    return Ok("Success");
                }
                else
                {
                    return Ok("Failed");
                }*/
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

            

            //var service = new PaymentIntentService();
            // var resultPayment = service.Create(optionsCreate);
            //resultPayment.SourceId = payment.SourceId;
            /*var options = new PaymentIntentConfirmOptions
            {
                PaymentMethod = "pm_card_visa",
            };*/
            //service.Confirm(resultPayment.Id, options);
            // service.Capture(resultPayment.Id);

            /*Dictionary<string, string> Metadata = new Dictionary<string, string>();
            Metadata.Add("Product", "RubberDuck");
            Metadata.Add("Quantity", "10");
            var customers = new CustomerService();*/
            // var charges = new ChargeService();
            /* var customer = customers.Create(new CustomerCreateOptions
             {
                 Email = stripeEmail,
                 Source = stripeToken
             });*/

            /*  var options = new PaymentIntentCreateOptions
              {
                  Amount = amount,
                  Currency = "usd",
                  ReceiptEmail = stripeEmail,
                  PaymentMethodTypes = new List<string>
                  { 
                     "card",
                  }
              };

              var service = new PaymentIntentService();
              service.Create(options);*/
            /*service.Confirm("pi_1DpRUj2eZvKYlo2CCSBrmuIc", options)*/

            /* if (service. == "succeeded")
             {
                 string balanceTransactionId = charge.BalanceTransactionId;
                 return Ok($"success, balanceTransactionId : {balanceTransactionId}");
             }*/
            //return Ok(service.Client.ClientId);
        }
    }
}

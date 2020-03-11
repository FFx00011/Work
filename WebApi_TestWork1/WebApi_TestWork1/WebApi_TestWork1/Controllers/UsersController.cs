using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApi_TestWork1.Data;
using WebApi_TestWork1.HelperClasses;
using WebApi_TestWork1.Models;
using WebApi_TestWork1.Utils;

namespace WebApi_TestWork1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDBContext _context;

        public UsersController(AppDBContext context)
        {
            _context = context;
        }
        // Get base info form
        [HttpGet]
        public IActionResult GetUser()
        {
            string info = "Get user balance GET: api/Users/{UserIdentityToken}\n" +
                            "Replenishment balance POST: api/Users/replenishment/{UserIdentityToken}  Json: { 'Valute': '', 'Amount':'0.00' }\n" +
                            "Conclusion balance POST: api/Users/conclusion/{UserIdentityToken}  Json: { 'Valute': '', 'Amount':'0.00' }\n" +
                            "Convert valute POST: api/Users/convert/{UserIdentityToken}  Json: { 'InValute': 'string', 'ToValute':'string', 'Amount':'decimal' }";
            return Ok(info);
        }
        // Get current user balance
        // GET: api/Users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("enter the user id ../api/Users/{id}");
            }
            var UserValutes = await _context.Users.Where(s => s.UserIdentityToken == id).ToListAsync();
         
            if (UserValutes == null)
            {
                return BadRequest("enter the user id ../api/Users/{id}");
            }
            return Ok(UserValutes);
        }
        // replenishment balance
        // POST: api/Users/replenishment/{id}  Json: { "Valute": "", "Amount":"" }
        [HttpPost("replenishment/{id}")]
        public async Task<IActionResult> replenishment([FromRoute] string id, [FromBody] replenishmentParameters _replenishmentParameters)
        {
            var _user = new User();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_replenishmentParameters.Valute == null | _replenishmentParameters.Amount <=0)
            {
                return BadRequest("Error Parameters");
            }
            var UserValutes = await _context.Users.Where(s => s.UserIdentityToken == id).ToListAsync();
            
            if (UserValutes == null | UserValutes.Count==0)
            {
                return BadRequest("Error user not found");
            }
            else
            {
                for(int i=0; i < UserValutes.Count(); i++)
                {
                    if(UserValutes[i].ValuteName== _replenishmentParameters.Valute)
                    {
                        try
                        {
                            UserValutes[i].ValuteCount += Convert.ToDecimal(_replenishmentParameters.Amount);
                            _user = UserValutes[i];
                            await _context.SaveChangesAsync();
                            return Ok("translation completed successfully");
                        } catch { }
                    }
                }
                _user.UserIdentityToken = id;
                _user.ValuteName = _replenishmentParameters.Valute;
                _user.ValuteCount = Convert.ToDecimal(_replenishmentParameters.Amount);
                _context.Users.Add(_user);
                await _context.SaveChangesAsync();
                return Ok("translation completed successfully");
            }
        }
        // conclusion balance
        // POST: api/Users/conclusion/{id}  Json: { "Valute": "", "Amount":"" }
        [HttpPost("conclusion/{id}")]
        public async Task<IActionResult> conclusion([FromRoute] string id, [FromBody] conclusionParameters _conclusionParameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_conclusionParameters.Valute == null | _conclusionParameters.Amount <=0)
            {
                return BadRequest("Error Parameters");
            }
            var UserValutes = await _context.Users.Where(s => s.UserIdentityToken == id).ToListAsync();

            if (UserValutes == null | UserValutes.Count == 0)
            {
                return BadRequest("Error! user not found");
            }
            else
            {
                for (int i = 0; i < UserValutes.Count(); i++)
                {
                    if (UserValutes[i].ValuteName == _conclusionParameters.Valute)
                    {
                        try
                        {
                            if(UserValutes[i].ValuteCount>= Convert.ToDecimal(_conclusionParameters.Amount))
                            {
                                UserValutes[i].ValuteCount -= Convert.ToDecimal(_conclusionParameters.Amount);
                                if (UserValutes[i].ValuteCount < 0) { UserValutes[i].ValuteCount = 0; }
                                await _context.SaveChangesAsync();
                                return Ok("translation completed successfully");
                            }
                        }
                        catch { }
                    }
                }
                return BadRequest("Error! insufficient funds in the indicated wallet with the specified currency");
            }
        }
        // СonvertСurrency 
        // POST: api/Users/convert/{id}  Json: { 'InValute': 'string', 'ToValute':'string', 'Amount':'decimal' }
        [HttpPost("convert/{id}")]
        public async Task<IActionResult> convert([FromRoute] string id, [FromBody] ValuteConvertParameters _ValuteConvertParameters)
        {
            if (!ModelState.IsValid | _ValuteConvertParameters.InValute ==null & _ValuteConvertParameters.ToValute==null | _ValuteConvertParameters.Amount<=0)
            {
                return BadRequest("Error parameters");
            }
           
            var AllRates = GetValuteRatesFromAPI.GetActualValuteRate();
            var UserValutes = await _context.Users.Where(s => s.UserIdentityToken == id).ToListAsync();
            decimal InValuteRate = 0;
            decimal ToValuteRate = 0;
            if (AllRates != null & UserValutes != null)
            {
                foreach (var _rate in AllRates)
                {
                    if (_rate.Name == _ValuteConvertParameters.InValute) { InValuteRate = _rate.Rate; }
                    if (_rate.Name == _ValuteConvertParameters.ToValute) { ToValuteRate = _rate.Rate; }
                }
                if (InValuteRate > 0 & ToValuteRate > 0)
                {
                    var NewCurrency = ValuteConverter.Convert(InValuteRate, _ValuteConvertParameters.Amount, ToValuteRate);
                    if (NewCurrency >= 0)
                    {
                        for (int i = 0; i < UserValutes.Count(); i++)
                        {
                            if (UserValutes[i].ValuteName == _ValuteConvertParameters.InValute)
                            {
                                if (UserValutes[i].ValuteCount >= _ValuteConvertParameters.Amount)
                                {
                                    UserValutes[i].ValuteCount -= _ValuteConvertParameters.Amount;
                                    if (UserValutes[i].ValuteCount < 0) { UserValutes[i].ValuteCount = 0; }
                                    for (int j = 0; j < UserValutes.Count(); j++)
                                    {
                                        // find valute in current user and encrement it value
                                        if (UserValutes[j].ValuteName == _ValuteConvertParameters.ToValute)
                                        {
                                            UserValutes[j].ValuteCount += NewCurrency;
                                            await _context.SaveChangesAsync();
                                            return Ok("translation completed successfully");
                                        }
                                    }
                                    // create new valute if not exist
                                    var _user = new User();
                                    _user.UserIdentityToken = id;
                                    _user.ValuteName = _ValuteConvertParameters.ToValute;
                                    _user.ValuteCount = Convert.ToDecimal(_ValuteConvertParameters.Amount);
                                    _context.Users.Add(_user);
                                    await _context.SaveChangesAsync();
                                    return Ok("translation completed successfully");
                                }
                            }
                        }
                    }
                    else { return BadRequest("Calc new valute amount Error."); }
                }
                else { return BadRequest("Valute rate not found."); }
            }
            if (AllRates == null) { return BadRequest("Error valute rates not found."); }
            if( UserValutes == null) { return BadRequest("Error current valute on current user not found."); }
            return BadRequest("Error");
        }
    }
}
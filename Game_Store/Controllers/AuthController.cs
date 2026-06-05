using Microsoft.AspNetCore.Mvc;
using Game_Store.Data; 
using Game_Store.Models; 
using Game_Store.DTOs; 
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Identity.Client;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Game_Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDataBaseContext _context;


        [HttpGet("Anasayfa")]

        public async Task<IActionResult> AnaSayfaOyunları()
        {


            try
            {
           
                var hamVeri = await _context.Database
                                            .SqlQuery<AnaSayfaDTO>($"EXEC sp_anasayfa")
                                            .ToListAsync();

                double dolarKuru = 45.97;

              
                var kuruHesaplanmisHamVeri = hamVeri.Select(o => new AnaSayfaDTO
                {
                    oyunadi = o.oyunadi,
                    incelemepuani = o.incelemepuani,

                    oyun_fiyatı = (o.platform_adi?.ToLower().Contains("steam") == true)
                                  ? Math.Round((o.oyun_fiyatı ?? 0) * dolarKuru, 2)
                                  : o.oyun_fiyatı,

                    platform_adi = o.platform_adi,

                    parabirimi = (o.platform_adi?.ToLower().Contains("steam") == true) ? "TL" : o.parabirimi
                }).ToList();


                var toparlanmisListe = kuruHesaplanmisHamVeri
                    .GroupBy(o => o.oyunadi)
                    .Select(g => new AnaSayfaDTO
                    {
                        oyunadi = g.Key,
                        dlc = g.First().dlc,
                        incelemepuani = g.First().incelemepuani,
                        oyun_fiyatı = g.Min(x => x.oyun_fiyatı),
                        parabirimi = "TL",


                        platform_adi = string.Join(" | ", g.Select(x => $"{x.platform_adi} ({x.oyun_fiyatı} TL)"))
                    })
                    .Take(5)
                    .ToList();

                return Ok(toparlanmisListe);
            
            }


            catch (Exception ex) 
            {
                return StatusCode(500,$"Hata oluştu:{ex.Message}");
            }
        }


        public AuthController(AppDataBaseContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegisterDTO request)
        {

            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest("Kullanıcı adı ve şifre boş bırakılamaz.");
            }

            try
            {
               
                _context.Database.ExecuteSqlRaw(
                    "EXEC sp_Kullanici_Kayit @p0, @p1,@p2",
                    request.Email,
                    request.Password,
                    request.Username
                );

                return Ok(new { Message = "Kayıt işlemi SQL Stored Procedure ile başarıyla tamamlandı!" });
            }
            catch (Exception ex)
            {
            

                return BadRequest("Kayıt başarısız: " + ex.Message);
            }

        }

        [HttpPost("login")]
        public IActionResult Giris([FromBody] UserLoginDTO request)
        {

            if(string.IsNullOrWhiteSpace(request.Email)|| string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email ve şifre boş bırakılamaz");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserName == request.Email && u.Password == request.Password);

            if(user==null)
            {
                return Unauthorized("Email veya şifre hatalı");
            }

            return Ok(new
            {
                Message = "Giriş başarılı",
                UserID = user.Id,
                Nick = user.UserName
            });

        }
        [HttpGet("DilDestekleri")]

        public async Task<IActionResult> Diller([FromQuery] string oyun_adi)
        {
            try
            {
                var diller = await _context.Database.SqlQuery<DilDTO>($"Exec Dilleri_goster {oyun_adi}").ToListAsync();

                return Ok(diller);
            }

            catch(Exception ex)
            {
                return StatusCode(500, $"Hata oluştu: {ex.Message}");
            }
          
        }


        [HttpPost("Search")]
        public async Task<IActionResult> OyunAra([FromQuery] string kelime)
        {
            if(string.IsNullOrEmpty(kelime))
            {
               return BadRequest("Arama kelimesi boş bırakılamaz");
            }


            try
            {
                var aramasonuclari = await _context.Database.SqlQuery<AramaDTO>($"EXEC sp_Aramakutusu {kelime}").ToListAsync();
                return Ok(aramasonuclari);
            }


            catch (Exception ex)
            {

                return StatusCode(500, $"Arama sırasında hata oluştu: {ex.Message }");
            }

          
        }



        [HttpPost("OyunTürleri")]

        public async Task <IActionResult> OyunTürleri([FromQuery] string butonadi)
        {
            string viewtürü = "";



            switch(butonadi.ToLower())
            {
                case "aksiyon":
                viewtürü = "v_Aksiyon_Oyunları";
                break;

                case "korku":
                viewtürü = "v_Korku_Oyunları";
                break;

                case "rpg":
                viewtürü = "v_RPG_Oyunları";
                break;
                case "2d":
                viewtürü = "v_2D_Oyunları";
                break;

                case "soulslike":
                viewtürü = "v_Soulslike_Oyunları";
                break;

                case "macera":
                    viewtürü = "v_Macera_Oyunları";
                    break;
                case "dövüş":
                    viewtürü = "v_Dövüş_Oyunları";
                    break;

            }



            try
            {
                var oyunlar = await _context.Database.SqlQueryRaw<TürDTO>($"SELECT * FROM {viewtürü}").ToListAsync();
                return Ok(oyunlar);

            }

            catch(Exception ex)
            {
                return StatusCode(500, $"Oyunlar getirilemedi: {ex.Message}");
            }

        }



        [HttpPost("/api/Auth/SepeteEkle")]
        public async Task<IActionResult>SepeteEkle([FromQuery]int gameid, [FromQuery] int kullaniciid)
        {

            if (kullaniciid < 3)
            {
                return Unauthorized("Sepete ürün eklemek için lütfen giriş yapın.");
            }

            try
            {
                var sonuclistesi = await _context.Database.SqlQueryRaw<IslemSonucu>(
                    "Exec Sepete_ekleme @user_id={0}, @game_id={1}", kullaniciid, gameid).ToListAsync();

                var sonuc = sonuclistesi.FirstOrDefault();
                return Ok(sonuc);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sepete ekleme başarısız: {ex.Message}");
            }
        }




        [HttpDelete("SepettenSil")]

        public async Task<IActionResult> SepettenSil([FromQuery]int gameid, [FromQuery] int kullanıcıid)
        {

            if(kullanıcıid<3)
            {
                return Unauthorized("Siteye giriş yapın lütfen");            }
            try
            {
                var sonuclistesi = await _context.Database.SqlQueryRaw<IslemSonucu>("Exec Sepetten_Silme @user_id={0}, @game_id={1}", kullanıcıid, gameid).ToListAsync();
                var sonuc = sonuclistesi.FirstOrDefault();
                return Ok(sonuc);
            }


            catch (Exception ex) 
            {
                return StatusCode(500, $"Sepete ekleme başarısız:{ex.Message}");

            }
        }
    }
}

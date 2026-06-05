using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace Game_Store.DTOs
{
    public class UserRegisterDTO
    {
        public string ?Username { get; set; }
        public string ?Password { get; set; }    
        public string ?Email { get; set; }
    }


    public class UserLoginDTO
    {
        public string ?Email { get; set; }
        public string ?Password { get; set; }
    }

     public class DilDTO
    {
        public string ?oyunadi { get; set; }
        public string ?altyazi { get; set; }
        public string ?dublaj { get; set; }
        public string ?arayüz { get; set; }
        public string ?dil_adi { get; set; }

        public short oyunid { get; set; }
    }

    public class AramaDTO
    {

        public short oyunid { get; set; }
        public string ?tür { get; set; }
        public string ?oyunadi { get; set; }

        public double ?oyun_fiyatı { get; set; }

        public string ?parabirimi { get; set; }

        public string ?platform_adi { get; set; }


    

    }


    public class TürDTO
    {
        public short oyunid { get; set; }
        public string oyunadi { get; set; }
        public string tür { get; set; }
        public string gelistirici_firma { get; set; }

        public byte incelemepuani { get; set; }

        public double  ?oyun_fiyatı { get; set; }

        public string ?parabirimi { get; set; }

        public string ?platform_adi { get; set; }
    }
    [Keyless]
    public class IslemSonucu
    {
        public string mesaj { get; set; }
        public string durum { get; set; }
    }
    public class SepetDTO
    {
        public byte kullanıcı_id { get; set; }    
        public byte game_id { get; set; }   

        public DateTime purchase_date { get; set; }

        public short sepet_id { get; set; } 
    }


    public class AnaSayfaDTO
    {
        public string ?oyunadi { get; set; }
        public string ?dlc { get; set; }
        public double ?oyun_fiyatı { get; set; }
       
        public string ?parabirimi { get; set; }
        public string ?platform_adi { get; set; }
        public byte ?incelemepuani { get; set; }
    }
}

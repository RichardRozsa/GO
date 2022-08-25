using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

namespace Go.Configuration
{
    public class Program
    {
        public const int MaxAka = 20;

        //[Required]
        public string SearchText { get; set; }
        public bool? SearchTextWithYear { get; set; }
        public bool? SearchTextWithoutYear { get; set; }
        public int ReleaseYear { get; set; }
        public string Comment { get; set; }
        public string ImdbUrl { get; set; }
        public string Aka1 { get; set; }
        public bool? Aka1WithYear { get; set; }
        public bool? Aka1WithoutYear { get; set; }
        public string Aka2 { get; set; }
        public bool? Aka2WithYear { get; set; }
        public bool? Aka2WithoutYear { get; set; }
        public string Aka3 { get; set; }
        public bool? Aka3WithYear { get; set; }
        public bool? Aka3WithoutYear { get; set; }
        public string Aka4 { get; set; }
        public bool? Aka4WithYear { get; set; }
        public bool? Aka4WithoutYear { get; set; }
        public string Aka5 { get; set; }
        public bool? Aka5WithYear { get; set; }
        public bool? Aka5WithoutYear { get; set; }
        public string Aka6 { get; set; }
        public bool? Aka6WithYear { get; set; }
        public bool? Aka6WithoutYear { get; set; }
        public string Aka7 { get; set; }
        public bool? Aka7WithYear { get; set; }
        public bool? Aka7WithoutYear { get; set; }
        public string Aka8 { get; set; }
        public bool? Aka8WithYear { get; set; }
        public bool? Aka8WithoutYear { get; set; }
        public string Aka9 { get; set; }
        public bool? Aka9WithYear { get; set; }
        public bool? Aka9WithoutYear { get; set; }
        public string Aka10 { get; set; }
        public bool? Aka10WithYear { get; set; }
        public bool? Aka10WithoutYear { get; set; }
        public string Aka11 { get; set; }
        public bool? Aka11WithYear { get; set; }
        public bool? Aka11WithoutYear { get; set; }
        public string Aka12 { get; set; }
        public bool? Aka12WithYear { get; set; }
        public bool? Aka12WithoutYear { get; set; }
        public string Aka13 { get; set; }
        public bool? Aka13WithYear { get; set; }
        public bool? Aka13WithoutYear { get; set; }
        public string Aka14 { get; set; }
        public bool? Aka14WithYear { get; set; }
        public bool? Aka14WithoutYear { get; set; }
        public string Aka15 { get; set; }
        public bool? Aka15WithYear { get; set; }
        public bool? Aka15WithoutYear { get; set; }
        public string Aka16 { get; set; }
        public bool? Aka16WithYear { get; set; }
        public bool? Aka16WithoutYear { get; set; }
        public string Aka17 { get; set; }
        public bool? Aka17WithYear { get; set; }
        public bool? Aka17WithoutYear { get; set; }
        public string Aka18 { get; set; }
        public bool? Aka18WithYear { get; set; }
        public bool? Aka18WithoutYear { get; set; }
        public string Aka19 { get; set; }
        public bool? Aka19WithYear { get; set; }
        public bool? Aka19WithoutYear { get; set; }
        public string Aka20 { get; set; }
        public bool? Aka20WithYear { get; set; }
        public bool? Aka20WithoutYear { get; set; }
        public List<Episode> Episodes { get; set; }
    }
}
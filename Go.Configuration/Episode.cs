using System;

namespace Go.Configuration
{
    public class Episode
    {
        public string Number { get; set; }

        public string Status { get; set; }

        //public EpisodeStatusTypes StatusEnum
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(Status))
        //            return EpisodeStatusTypes.Search;

        //        EpisodeStatusTypes result;
        //        if (!Enum.TryParse(Status, true, out result))
        //            throw new Exception(string.Format("Unknown Episode status: {0}", Status));
        //        return result;
        //    }
        //}

        public string CheckPeriod { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public string ReleaseDateFormatNumber { get; set; }
    }
}
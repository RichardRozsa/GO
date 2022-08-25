
using Go.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Go
{
	public class ProgramList : IEnumerable<string>
    {
        private const int MonthlyCheckDay = 1;

		public ProgramList(List<Program> programs, bool quiet = true, bool monthlyOverride = false)
		{
			Programs = programs;
            Quiet = quiet;
            MonthlyOverride = monthlyOverride;
            Reset();
		}

        public IEnumerator<string> GetEnumerator()
        {
            Reset();

            //MessageBox.Show($"Programs = {Programs.Count}");
            foreach (var program in Programs)
            {
                if (!Quiet)
                    MessageBox.Show($"Processing program: {program.SearchText}");
                ProgramCount++;

                if (program.ReleaseYear > DateTime.Now.Year)
                {
                    if (!Quiet)
                        MessageBox.Show($"Continuing to next ReleaseYear {program.SearchText}");
                    continue;
                }

                for (var akaNr = 0; akaNr <= Program.MaxAka; akaNr++)
                {
                    // Get each SearchText and Aka# text
                    var text = GetAkaText(program, akaNr, out var withYear, out var withoutYear);
                    // Skip empty text
                    if (string.IsNullOrEmpty(text))
                    {
                        if (!Quiet)
                            MessageBox.Show($"Exiting Aka at nr. {akaNr}");
                        break;
                    }

                    AkaCount++;

                    // Skip disabled text (that starts with "~")
                    if (text.StartsWith("~"))
                    {
                        DisabledCount++;
                        if (!Quiet)
                            MessageBox.Show($"Exiting with disabled {text}");
                        continue;
                    }

                    if (program.Episodes == null || program.Episodes.Count == 0)
                    {
                        if (withYear && program.ReleaseYear > 0)
                        {
                            CurrentCount++;
                            if (!Quiet)
                                MessageBox.Show($"Returning 1 {text}");
                            yield return FormatSearchText(text, program.ReleaseYear, null);
                        }
                        if (withoutYear)
                        {
                            CurrentCount++;
                            if (!Quiet)
                                MessageBox.Show($"Returning 2 {text}");
                            yield return FormatSearchText(text, 0, null);
                        }
                    }
                    else
                    {
                        var epNr = 0;
                        foreach (var episode in program.Episodes)
                        {
                            epNr++;
                            if (!Quiet)
                                MessageBox.Show($"Processing episode nr. {epNr} for {text}");

                            EpisodeCount++;

                            // Skip null episodes (probably not possible)
                            if (episode == null)
                            {
                                if (!Quiet)
                                    MessageBox.Show($"Continuing episodes null episode {text}");
                                continue;
                            }
                            //MessageBox.Show("1");
                            // If the Status is defined, skip every status except "Search"
                            if (!string.IsNullOrEmpty(episode.Status) && episode.Status != "Search")
                            {
                                if (!Quiet)
                                    MessageBox.Show($"Continuing episodes non Search status {text}");
                                continue;
                            }
                            //MessageBox.Show("2");
                            // If the CheckPeriod is defined, skip if not "Monthly" or if it's not currently the MonthlyCheckDay
                            if (!string.IsNullOrEmpty(episode.CheckPeriod)
                                && (episode.CheckPeriod != "Monthly" || (DateTime.Now.Day != MonthlyCheckDay && !MonthlyOverride)))
                            {
                                if (!Quiet)
                                    MessageBox.Show($"Continuing episodes monthly check period {text}");
                                continue;
                            }
                            //MessageBox.Show("3");
                            // If the ReleaseDate is defined, skip if it's later than the current date.
                            if (episode.ReleaseDate.HasValue && episode.ReleaseDate.Value > DateTime.Now)
                            {
                                if (!Quiet)
                                    MessageBox.Show($"Continuing episodes releaseDate {text}");
                                continue;
                            }

                            //MessageBox.Show($"4: WithYear:{withYear} : WithoutYear:{withoutYear} : ReleaseYear:{program.ReleaseYear}");
                            if (withYear && program.ReleaseYear > 0)
                            {
                                //MessageBox.Show("5");
                                CurrentCount++;
                                if (!Quiet)
                                    MessageBox.Show($"Returning 3: Release Year {text}");
                                yield return FormatSearchText(text, program.ReleaseYear, episode);
                            }
                            //MessageBox.Show("6");
                            if (withoutYear)
                            {
                                //MessageBox.Show("7");
                                CurrentCount++;
                                if (!Quiet)
                                    MessageBox.Show($"Returning 4: Without Release Year {text}");
                                yield return FormatSearchText(text, 0, episode);
                            }
                        }
                    }
                }
            }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

        private string GetAkaText(Program program, int akaNr, out bool withYear, out bool withoutYear)
        {
            switch (akaNr)
            {
                case 0:
                    withYear = program.SearchTextWithYear ?? true;
                    withoutYear = program.SearchTextWithoutYear ?? true;
                    return program.SearchText;
                case 1:
                    withYear = program.Aka1WithYear ?? true;
                    withoutYear = program.Aka1WithoutYear ?? true;
                    return program.Aka1;
                case 2:
                    withYear = program.Aka2WithYear ?? true;
                    withoutYear = program.Aka2WithoutYear ?? true;
                    return program.Aka2;
                case 3:
                    withYear = program.Aka3WithYear ?? true;
                    withoutYear = program.Aka3WithoutYear ?? true;
                    return program.Aka3;
                case 4:
                    withYear = program.Aka4WithYear ?? true;
                    withoutYear = program.Aka4WithoutYear ?? true;
                    return program.Aka4;
                case 5:
                    withYear = program.Aka5WithYear ?? true;
                    withoutYear = program.Aka5WithoutYear ?? true;
                    return program.Aka5;
                case 6:
                    withYear = program.Aka6WithYear ?? true;
                    withoutYear = program.Aka6WithoutYear ?? true;
                    return program.Aka6;
                case 7:
                    withYear = program.Aka7WithYear ?? true;
                    withoutYear = program.Aka7WithoutYear ?? true;
                    return program.Aka7;
                case 8:
                    withYear = program.Aka8WithYear ?? true;
                    withoutYear = program.Aka8WithoutYear ?? true;
                    return program.Aka8;
                case 9:
                    withYear = program.Aka9WithYear ?? true;
                    withoutYear = program.Aka9WithoutYear ?? true;
                    return program.Aka9;
                case 10:
                    withYear = program.Aka10WithYear ?? true;
                    withoutYear = program.Aka10WithoutYear ?? true;
                    return program.Aka10;
                case 11:
                    withYear = program.Aka11WithYear ?? true;
                    withoutYear = program.Aka11WithoutYear ?? true;
                    return program.Aka11;
                case 12:
                    withYear = program.Aka12WithYear ?? true;
                    withoutYear = program.Aka12WithoutYear ?? true;
                    return program.Aka12;
                case 13:
                    withYear = program.Aka13WithYear ?? true;
                    withoutYear = program.Aka13WithoutYear ?? true;
                    return program.Aka13;
                case 14:
                    withYear = program.Aka14WithYear ?? true;
                    withoutYear = program.Aka14WithoutYear ?? true;
                    return program.Aka14;
                case 15:
                    withYear = program.Aka15WithYear ?? true;
                    withoutYear = program.Aka15WithoutYear ?? true;
                    return program.Aka15;
                case 16:
                    withYear = program.Aka16WithYear ?? true;
                    withoutYear = program.Aka16WithoutYear ?? true;
                    return program.Aka16;
                case 17:
                    withYear = program.Aka17WithYear ?? true;
                    withoutYear = program.Aka17WithoutYear ?? true;
                    return program.Aka17;
                case 18:
                    withYear = program.Aka18WithYear ?? true;
                    withoutYear = program.Aka18WithoutYear ?? true;
                    return program.Aka18;
                case 19:
                    withYear = program.Aka19WithYear ?? true;
                    withoutYear = program.Aka19WithoutYear ?? true;
                    return program.Aka19;
                case 20:
                    withYear = program.Aka20WithYear ?? true;
                    withoutYear = program.Aka20WithoutYear ?? true;
                    return program.Aka20;
                default:
                    throw new Exception($"Unsupported AkaNr {akaNr}");
            }
        }

        private string FormatSearchText(string searchText, int releaseYear, Episode episode)
        {
            var sb = new StringBuilder();
            sb.Append(searchText);
            if (releaseYear != 0)
                sb.Append($" {releaseYear}");

            if (episode != null)
            {
                var number = episode.Number;

                // Format ReleaseDateFormatNumber
                if (!string.IsNullOrEmpty(episode.ReleaseDateFormatNumber) && episode.ReleaseDate.HasValue)
                {
                    try
                    {
                        if (episode.ReleaseDateFormatNumber.Contains("{"))
                            number = string.Format(episode.ReleaseDateFormatNumber, episode.ReleaseDate.Value);
                        else
                            number = episode.ReleaseDateFormatNumber;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Program: {0}, Episode Format Number: {1}, Release Date: {2:yyyy-MM-dd}, Exception: {3}", searchText, episode.ReleaseDateFormatNumber, episode.ReleaseDate, ex.Message));
                    }
                }

                if (!string.IsNullOrEmpty(number))
                    sb.Append($" {number}");
            }

            return sb.ToString();
        }

        private void Reset()
        {
            ProgramCount = 0;
            DisabledCount = 0;
            AkaCount = 0;
            EpisodeCount = 0;
            CurrentCount = 0;
        }

        private List<Program> Programs { get; }
        private bool MonthlyOverride { get; }
        private bool Quiet { get; }
        public int ProgramCount { get; set; }
        public int DisabledCount { get; set; }
        public int AkaCount { get; set; }
        public int EpisodeCount { get; set; }
        public int CurrentCount { get; set; }
        public int Count { get { return CurrentCount; } }
    }
}

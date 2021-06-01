using Newtonsoft.Json;
using System.Threading;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http;
using Telegram.Bot;
using System.Text;
using System.Collections.Generic;

namespace tg_bot
{
    class Program
    {
        private static readonly TelegramBotClient botclient = new TelegramBotClient("1758931408:AAFU32b8YlOq9YWJkFIRcd2Qo8JA4BUtxoU");
        static void Main(string[] args)
        {
            
            botclient.OnMessage += Botclient_OnMessage;
            botclient.OnMessageEdited += Botclient_OnMessage;

            botclient.StartReceiving();
            Console.ReadLine();
            botclient.StopReceiving();
        }

        private static void Botclient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            try { 
            HttpClient clientAPI = new HttpClient();
            clientAPI.BaseAddress = new Uri("https://moviehelperapi.azurewebsites.net");
           
            HttpClient _client = new HttpClient();
            string _adress = "https://moviehelperapi.azurewebsites.net/api/MovieSearcher";
                if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
                {
                    int k = 0;
                    if (e.Message.Text == "/start")
                    {
                        Random rnd = new Random();
                        k = rnd.Next(1, 100);
                        botclient.SendTextMessageAsync(e.Message.Chat.Id, $"Hi, dear user!!! \nYou use telegram bot that can help you to choose film on today on tomorrow and on evry day. Also I can advice you film or you can filtr a lot list of them. Also you can add some films to favorites and watch them later\nBut please remember your special ID is {k}. So, go start...\nFor begining enter /go");
                    }
                    else if (e.Message.Text == "/go") botclient.SendTextMessageAsync(e.Message.Chat.Id, "Ok go start)\nIt`s some information how to work with this telegram bot. So, you can see some key words that can help you. But be very attentive, when you will enter new command\nEnter \"/random_film\"  if you want to get random film. But if you don`t want to watch this film you can enter \"/random_film\" one more\nEnter \"/filtr\"  if you want to filtr list of films by categories\nEnter \"/add\"  if you want to add film to your list of favorites\nEnter \"/film_that_can_me_like:<your personal ID>\" if you want film that is similar on films from your list of favorites\nEnter \"/film_from_year:<year that you want>\" if you want film from concrete year\nEnter \"/film_from_years:<from>-<to>\" if you want film from concrete 10 years\nEnter \"/film_from_genre:<genre of film>\" if you want film from concrete genre\nEnter \"/popular_film_on_today\" if you want to see what film a popular today\nEnter \"/favorites\" if you want to see list of your favorites\nEnter \"/getfilmsBygenre_fromIMDb:<genre>\" if you want get 3 films by genre from list of IMDb");
                    else if (e.Message.Text == "/random_film")
                    {
                        int n = 20;
                        ranF film = new ranF();
                        random_film(_adress, ref film);
                        int_favor(ref n);
                        film.data.movie.id = ++n;

                        botclient.SendPhotoAsync(e.Message.Chat.Id, film.data.movie.large_cover_image);
                        Thread.Sleep(1000);
                        string genres = "";
                        for (int i = 0; i < film.data.movie.genres.Count; i++)
                        {
                            genres += film.data.movie.genres[i];
                            genres += ", ";
                        }
                        genres = genres.Substring(0, genres.Length - 2);
                        botclient.SendTextMessageAsync(e.Message.Chat.Id, $"{film.data.movie.title_long}\n\nyear - {film.data.movie.year} \nruntime:{film.data.movie.runtime} minutes\nrating of this film: {film.data.movie.rating}\n\ngenres of this film: {genres}\n\nDescription of this film:{film.data.movie.description_full}\nlanguage of this film on site: {film.data.movie.language}\ncount of likes: {film.data.movie.like_count}\n\nYou can download this film by using this link: {film.data.movie.Url}\nSpecial ID of this film if you want to add this film to the list of favorites: {film.data.movie.id}");
                        film_post Film = new film_post();
                        Film.Id = Convert.ToString(film.data.movie.id);
                        Film.UserId = "0";
                        Film.Name = film.data.movie.title_long;
                        Film.genre = film.data.movie.genres[0];
                        Film.description_full = film.data.movie.description_full;
                        Film.large_cover_image = film.data.movie.large_cover_image;
                        Film.runtime = Convert.ToString(film.data.movie.runtime);
                        Film.url = film.data.movie.Url;
                        Film.year = Convert.ToString(film.data.movie.year);
                        addAsync(Film, clientAPI);
                    }
                    else if (e.Message.Text == "/filtr")
                    {
                        botclient.SendTextMessageAsync(e.Message.Chat.Id, "Enter count of films that you want to get\nEnter minimal rating of films that you want to get\nEnter genre of films that you want to get. you can see examples of genres lower\n\"comedy\"\n\"horror\"\n\"action\"\n\"romance\"\n\"thriller\"\n\"drama\"\n\"mystery\"\n\"crime\"\n\"animation\"\n\"adventure\"\n\"fantasy\"\n\"superhero\"");
                        botclient.SendTextMessageAsync(e.Message.Chat.Id, "But you must enter these wordss such you can see example:\n\n4->6,8->comedy");
                    }
                    else if (e.Message.Text.Contains('>') && e.Message.Text.Contains('-') && !e.Message.Text.Contains('<'))
                    {
                        string kk = e.Message.Text;
                        list_of_movies film1 = new list_of_movies();
                        int count;

                        int n = 0;
                        int_favor(ref n);
                        double mr = 1;
                        string genre = "";
                        count = Int32.Parse(kk.Substring(0, kk.IndexOf('>') - 1));
                        genre = kk.Substring(kk.LastIndexOf('>') + 1, kk.Length - 1 - (kk.LastIndexOf('>')));
                        kk = kk.Substring(kk.IndexOf('>') + 1, kk.Length - 1 - kk.IndexOf('>'));
                        mr = Convert.ToDouble(kk.Substring(0, kk.IndexOf('>') - 1));
                        list_of_films(_adress, count, mr, genre, ref film1);

                        for (int i = 0; i < film1.Data.Movies.Count; i++)
                        {
                            string genres = "";
                            for (int j = 0; j < film1.Data.Movies[i].Genres.Count; j++)
                            {
                                genres += film1.Data.Movies[i].Genres[j];
                                genres += ", ";
                            }
                            genres = genres.Substring(0, genres.Length - 2);
                            film1.Data.Movies[i].Id = ++n;
                            botclient.SendPhotoAsync(e.Message.Chat.Id, film1.Data.Movies[i].Large_cover_image);
                            Thread.Sleep(600);
                            botclient.SendTextMessageAsync(e.Message.Chat.Id, $"{film1.Data.Movies[i].Title_long}\n\nyear - {film1.Data.Movies[i].Year}           runtime:{film1.Data.Movies[i].Runtime} minutes\t\t\t\trating of this film: {film1.Data.Movies[i].Rating}\ngenres of this film:{genres}\n\nDescription of this film:{film1.Data.Movies[i].Description_full}\nlanguage of this film on site: {film1.Data.Movies[i].Language}\n\nYou can download this film by using this link: {film1.Data.Movies[i].Url}\nSpecial Id if you want add this film to list of favorites={film1.Data.Movies[i].Id}");
                            Thread.Sleep(1000);

                            film_post Film = new film_post();
                            Film.Id = Convert.ToString(film1.Data.Movies[i].Id);
                            Film.UserId = "0";
                            Film.Name = film1.Data.Movies[i].Title_long;
                            Film.genre = film1.Data.Movies[i].Genres[0];
                            Film.description_full = film1.Data.Movies[i].Description_full;
                            Film.large_cover_image = film1.Data.Movies[i].Large_cover_image;
                            Film.runtime = Convert.ToString(film1.Data.Movies[i].Runtime);
                            Film.url = film1.Data.Movies[i].Url;
                            Film.year = Convert.ToString(film1.Data.Movies[i].Year);
                            addAsync(Film, clientAPI);
                        }
                    }
                    else if (e.Message.Text == "/add")
                    {
                        botclient.SendTextMessageAsync(e.Message.Chat.Id, "Enter your special code and Id of film that you want to add\nExample: 1<->12");
                    }
                    else if (e.Message.Text.Contains('<') && e.Message.Text.Contains('-') && e.Message.Text.Contains('>'))
                    {
                        string IDD = e.Message.Text.Substring(e.Message.Text.LastIndexOf('>') + 1, e.Message.Text.Length - 1 - e.Message.Text.LastIndexOf('>'));
                        string Per_ID = e.Message.Text.Substring(0, e.Message.Text.IndexOf('<'));

                        addfavorit(IDD, Per_ID, clientAPI);
                    }
                    else if (e.Message.Text.Contains("/film_that_can_me_like"))
                    {
                        string IDD = e.Message.Text.Substring(e.Message.Text.LastIndexOf(':') + 1, e.Message.Text.Length - e.Message.Text.LastIndexOf(':') - 1);
                        Console.WriteLine(IDD);
                        list_of_movies film1 = new list_of_movies();
                        GetFavorites(IDD, clientAPI, ref film1);
                        Random rndo = new Random();
                        int value = rndo.Next(0, 30);
                        string genres = "";
                        for (int j = 0; j < film1.Data.Movies[value].Genres.Count; j++)
                        {
                            genres += film1.Data.Movies[value].Genres[j];
                            genres += ", ";
                        }
                        genres = genres.Substring(0, genres.Length - 2);
                        int n = 0;
                        int_favor(ref n);
                        film1.Data.Movies[value].Id = ++n;
                        botclient.SendPhotoAsync(e.Message.Chat.Id, film1.Data.Movies[value].Large_cover_image);
                        Thread.Sleep(900);
                        botclient.SendTextMessageAsync(e.Message.Chat.Id, $"{film1.Data.Movies[value].Title_long}\n\nyear - {film1.Data.Movies[value].Year}\nruntime:{film1.Data.Movies[value].Runtime} minutes\nrating of this film: {film1.Data.Movies[value].Rating}\ngenres of this film:{genres}\n\nDescription of this film:{film1.Data.Movies[value].Description_full}\nlanguage of this film on site: {film1.Data.Movies[value].Language}\n\nYou can download this film by using this link: {film1.Data.Movies[value].Url}\nSpecial Id={film1.Data.Movies[value].Id} if you want to add this film to list of favorites");
                        Thread.Sleep(1000);

                        film_post Film = new film_post();
                        Film.Id = Convert.ToString(film1.Data.Movies[value].Id);
                        Film.UserId = "0";
                        Film.Name = film1.Data.Movies[value].Title_long;
                        Film.genre = film1.Data.Movies[value].Genres[0];
                        Film.description_full = film1.Data.Movies[value].Description_full;
                        Film.large_cover_image = film1.Data.Movies[value].Large_cover_image;
                        Film.runtime = Convert.ToString(film1.Data.Movies[value].Runtime);
                        Film.url = film1.Data.Movies[value].Url;
                        Film.year = Convert.ToString(film1.Data.Movies[value].Year);
                        addAsync(Film, clientAPI);
                    }
                    else if (e.Message.Text.Contains("/film_from_year:"))
                    {
                        string IDD = e.Message.Text.Substring(e.Message.Text.LastIndexOf(':') + 1, e.Message.Text.Length - e.Message.Text.LastIndexOf(':') - 1);
                        int year = Convert.ToInt32(IDD);
                        ranF film = new ranF();
                        film_from_year(year, ref film);

                        int n = 0;
                        int_favor(ref n);
                        film.data.movie.id = ++n;
                        botclient.SendPhotoAsync(e.Message.Chat.Id, film.data.movie.large_cover_image);
                        Thread.Sleep(1400);
                        string genres = "";
                        for (int i = 0; i < film.data.movie.genres.Count; i++)
                        {
                            genres += film.data.movie.genres[i];
                            genres += ", ";
                        }
                        genres = genres.Substring(0, genres.Length - 2);
                        botclient.SendTextMessageAsync(e.Message.Chat.Id, $"{film.data.movie.title_long}\n\nyear - {film.data.movie.year} \nruntime:{film.data.movie.runtime} minutes\nrating of this film: {film.data.movie.rating}\n\ngenres of this film: {genres}\n\nDescription of this film:{film.data.movie.description_full}\nlanguage of this film on site: {film.data.movie.language}\ncount of likes: {film.data.movie.like_count}\n\nYou can download this film by using this link: {film.data.movie.Url}\nSpecial ID of this film if you want to add this film to the list of favorites: {film.data.movie.id}");
                        film_post Film = new film_post();
                        Film.Id = Convert.ToString(film.data.movie.id);
                        Film.UserId = "0";
                        Film.Name = film.data.movie.title_long;
                        Film.genre = film.data.movie.genres[0];
                        Film.description_full = film.data.movie.description_full;
                        Film.large_cover_image = film.data.movie.large_cover_image;
                        Film.runtime = Convert.ToString(film.data.movie.runtime);
                        Film.url = film.data.movie.Url;
                        Film.year = Convert.ToString(film.data.movie.year);
                        addAsync(Film, clientAPI);
                    }
                    else if (e.Message.Text.Contains("film_from_years:"))
                    {
                        string IDD = e.Message.Text.Substring(e.Message.Text.LastIndexOf('-') + 1, e.Message.Text.Length - e.Message.Text.LastIndexOf('-') - 1);
                        int year2 = Convert.ToInt32(IDD);
                        IDD = e.Message.Text.Substring(e.Message.Text.LastIndexOf(':') + 1, e.Message.Text.Length - 1 - e.Message.Text.LastIndexOf(':') - 1);
                        string IDD1 = IDD.Substring(0, IDD.LastIndexOf('-'));
                        int year1 = Convert.ToInt32(IDD1);
                        ranF film = new ranF();
                        film_from_years(year1, year2, ref film);

                        int n = 0;
                        int_favor(ref n);
                        botclient.SendPhotoAsync(e.Message.Chat.Id, film.data.movie.large_cover_image);
                        Thread.Sleep(1400);
                        string genres = "";
                        for (int i = 0; i < film.data.movie.genres.Count; i++)
                        {
                            genres += film.data.movie.genres[i];
                            genres += ", ";
                        }
                        film.data.movie.id = ++n;
                        genres = genres.Substring(0, genres.Length - 2);
                        botclient.SendTextMessageAsync(e.Message.Chat.Id, $"{film.data.movie.title_long}\n\nyear - {film.data.movie.year} \nruntime:{film.data.movie.runtime} minutes\nrating of this film: {film.data.movie.rating}\n\ngenres of this film: {genres}\n\nDescription of this film:{film.data.movie.description_full}\nlanguage of this film on site: {film.data.movie.language}\ncount of likes: {film.data.movie.like_count}\n\nYou can download this film by using this link: {film.data.movie.Url}\nSpecial ID of this film if you want to add this film to the list of favorites: {film.data.movie.id}");
                        film_post Film = new film_post();
                        Film.Id = Convert.ToString(film.data.movie.id);
                        Film.UserId = "0";
                        Film.Name = film.data.movie.title_long;
                        Film.genre = film.data.movie.genres[0];
                        Film.description_full = film.data.movie.description_full;
                        Film.large_cover_image = film.data.movie.large_cover_image;
                        Film.runtime = Convert.ToString(film.data.movie.runtime);
                        Film.url = film.data.movie.Url;
                        Film.year = Convert.ToString(film.data.movie.year);
                        addAsync(Film, clientAPI);
                    }
                    else if (e.Message.Text.Contains("/film_from_genre:"))
                    {
                        string IDD = e.Message.Text.Substring(e.Message.Text.LastIndexOf(':') + 1, e.Message.Text.Length - e.Message.Text.LastIndexOf(':') - 1);
                        ranF film = new ranF();
                        film_from_genre(IDD, ref film);

                        int n = 0;
                        int_favor(ref n);
                        botclient.SendPhotoAsync(e.Message.Chat.Id, film.data.movie.large_cover_image);
                        Thread.Sleep(1400);
                        string genres = "";
                        for (int i = 0; i < film.data.movie.genres.Count; i++)
                        {
                            genres += film.data.movie.genres[i];
                            genres += ", ";
                        }
                        genres = genres.Substring(0, genres.Length - 2);
                        botclient.SendTextMessageAsync(e.Message.Chat.Id, $"{film.data.movie.title_long}\n\nyear - {film.data.movie.year} \nruntime:{film.data.movie.runtime} minutes\nrating of this film: {film.data.movie.rating}\n\ngenres of this film: {genres}\n\nDescription of this film:{film.data.movie.description_full}\nlanguage of this film on site: {film.data.movie.language}\ncount of likes: {film.data.movie.like_count}\n\nYou can download this film by using this link: {film.data.movie.Url}\nSpecial ID of this film if you want to add this film to the list of favorites: {film.data.movie.id}");
                        film_post Film = new film_post();
                        Film.Id = Convert.ToString(film.data.movie.id);
                        Film.UserId = "0";
                        Film.Name = film.data.movie.title_long;
                        Film.genre = film.data.movie.genres[0];
                        Film.description_full = film.data.movie.description_full;
                        Film.large_cover_image = film.data.movie.large_cover_image;
                        Film.runtime = Convert.ToString(film.data.movie.runtime);
                        Film.url = film.data.movie.Url;
                        Film.year = Convert.ToString(film.data.movie.year);
                        addAsync(Film, clientAPI);
                    }
                    else if (e.Message.Text.Contains("/popular_film_on_today"))
                    {
                        List<Movies> film2 = new List<Movies>();
                        //list_of_movies list = new list_of_movies();
                        list_of_movies film1 = new list_of_movies();
                        popular_list(_adress, 15, 8, ref film1, ref film2);
                        for (int i = 0; i < film2.Count; i++)
                        {
                            int n = 0;
                            int_favor(ref n);
                            string genres = "";
                            for (int j = 0; j < film2[i].Genres.Count; j++)
                            {
                                genres += film2[i].Genres[j];
                                genres += ", ";
                            }
                            film2[i].Id = ++n;
                            genres = genres.Substring(0, genres.Length - 2);
                            botclient.SendPhotoAsync(e.Message.Chat.Id, film2[i].Large_cover_image);
                            Thread.Sleep(800);
                            botclient.SendTextMessageAsync(e.Message.Chat.Id, $"{film2[i].Title_long}\n\nyear - {film2[i].Year}\nruntime:{film2[i].Runtime} minutes\nrating of this film: {film2[i].Rating}\ngenres of this film:{genres}\n\nDescription of this film:{film2[i].Description_full}\nlanguage of this film on site: {film2[i].Language}\n\nYou can download this film by using this link: {film2[i].Url}\nSpecial Id={film2[i].Id} if you want to add this film to list of favorites");
                            Thread.Sleep(600);
                            film_post Film = new film_post();
                            Film.Id = Convert.ToString(film2[i].Id);
                            Film.UserId = "0";
                            Film.Name = film2[i].Title_long;
                            Film.genre = film2[i].Genres[0];
                            Film.description_full = film2[i].Description_full;
                            Film.large_cover_image = film2[i].Large_cover_image;
                            Film.runtime = Convert.ToString(film2[i].Runtime);
                            Film.url = film2[i].Url;
                            Film.year = Convert.ToString(film2[i].Year);
                            addAsync(Film, clientAPI);
                        }
                    }
                    else if (e.Message.Text.Contains("/favorites"))
                    {
                        string IDD = e.Message.Text.Substring(e.Message.Text.LastIndexOf(':') + 1, e.Message.Text.Length - e.Message.Text.LastIndexOf(':') - 1);
                        string adress = $"https://moviehelperapi.azurewebsites.net/api/MovieSearcher/all_favorites?userId={IDD}";
                        string response1;
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(adress);
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (StreamReader st = new StreamReader(httpWebResponse.GetResponseStream()))
                        {
                            response1 = st.ReadToEnd();
                        }
                        var res = JsonConvert.DeserializeObject<List<film_post>>(response1);
                        for (int i = 0; i < res.Count; i++)
                        {
                            botclient.SendPhotoAsync(e.Message.Chat.Id, res[i].large_cover_image);
                            Thread.Sleep(900);
                            botclient.SendTextMessageAsync(e.Message.Chat.Id, $"{res[i].Name}\n\nyear - {res[i].year}\nruntime:{res[i].runtime} minutes\ngenres of this film:{res[i].genre}\n\nDescription of this film:{res[i].description_full}\n\nYou can download this film by using this link: {res[i].url}");
                            Thread.Sleep(1000);
                        }
                    }
                    else if (e.Message.Text.Contains("/getfilmsBygenre_fromIMDb:"))
                    {
                        string IDD = e.Message.Text.Substring(e.Message.Text.LastIndexOf(':') + 1, e.Message.Text.Length - e.Message.Text.LastIndexOf(':') - 1);
                        IMDb film1 = new IMDb();
                        IMDb film2 = new IMDb();
                        IMDb film3 = new IMDb();
                        int n = 0;
                        int_favor(ref n);
                        getIMDb(IDD, ref film1, ref film2, ref film3);

                        botclient.SendPhotoAsync(e.Message.Chat.Id, film1.Poster);
                        Thread.Sleep(1000);
                        botclient.SendTextMessageAsync(e.Message.Chat.Id, $"{film1.Title}\n\n year - {film1.Year}\nruntime:{film1.Runtime}\ngenres of this film:{film1.Genre}\n\nDescription of this film:{film1.Plot}\n\nIMDb rating of this film:{film1.imdbRating}\nActors:{film1.Actors}");
                        Thread.Sleep(500);
                        film_post Film = new film_post();
                        Film.Id = Convert.ToString(++n);
                        Film.UserId = "0";
                        Film.Name = film1.Title;
                        Film.genre = film1.Genre.Substring(0, film1.Genre.IndexOf(','));
                        Film.description_full = film1.Plot;
                        Film.large_cover_image = film1.Poster;
                        Film.runtime = film1.Runtime;
                        Film.url = "no link";
                        Film.year = film1.Year;
                        addAsync(Film, clientAPI);

                        botclient.SendPhotoAsync(e.Message.Chat.Id, film2.Poster);
                        Thread.Sleep(1000);
                        botclient.SendTextMessageAsync(e.Message.Chat.Id, $"{film2.Title}\n\n year - {film2.Year}\nruntime:{film2.Runtime}\ngenres of this film:{film2.Genre}\n\nDescription of this film:{film2.Plot}\n\nIMDb rating of this film:{film2.imdbRating}\nActors:{film2.Actors}");
                        Thread.Sleep(500);

                        film_post Film1 = new film_post();
                        Film1.Id = Convert.ToString(++n);
                        Film1.UserId = "0";
                        Film1.Name = film2.Title;
                        Film1.genre = film2.Genre.Substring(0, film2.Genre.IndexOf(','));
                        Film1.description_full = film2.Plot;
                        Film1.large_cover_image = film2.Poster;
                        Film1.runtime = film2.Runtime;
                        Film1.url = "no link";
                        Film1.year = film2.Year;
                        addAsync(Film1, clientAPI);

                        botclient.SendPhotoAsync(e.Message.Chat.Id, film3.Poster);
                        Thread.Sleep(1000);
                        botclient.SendTextMessageAsync(e.Message.Chat.Id, $"{film3.Title}\n\n year - {film3.Year}\nruntime:{film3.Runtime}\ngenres of this film:{film3.Genre}\n\nDescription of this film:{film3.Plot}\n\nIMDb rating of this film:{film3.imdbRating}\nActors:{film3.Actors}");
                        Thread.Sleep(500);
                        film_post Film3 = new film_post();
                        Film3.Id = Convert.ToString(++n);
                        Film3.UserId = "0";
                        Film3.Name = film3.Title;
                        Film3.genre = film3.Genre.Substring(0, film3.Genre.IndexOf(','));
                        Film3.description_full = film3.Plot;
                        Film3.large_cover_image = film3.Poster;
                        Film3.runtime = film3.Runtime;
                        Film3.url = "no link";
                        Film3.year = film3.Year;
                        addAsync(Film3, clientAPI);

                    }
                    else
                    {
                        botclient.SendPhotoAsync(e.Message.Chat.Id, "https://www.lingvaflavor.com/wp-content/uploads/2015/06/Pictures_4412399.jpg");
                        Thread.Sleep(800);
                        botclient.SendTextMessageAsync(e.Message.Chat.Id, "You entered wrong command. Please, check your request one more");
                    }
                }
                
            }
            catch
            {
                Console.WriteLine("exeption");
            }
        }


        public static void getIMDb(string genre, ref IMDb film1, ref IMDb film2, ref IMDb film3)
        {
            string adress1 = $"https://moviehelperapi.azurewebsites.net/api/MovieSearcher/bygenre_from_IMDb?genre={genre}";
            string response1;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(adress1);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader st = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response1 = st.ReadToEnd();
            }
            var film = JsonConvert.DeserializeObject<List<string>>(response1);
            Random rnd = new Random();
            int value11 = rnd.Next(0, 25);
            int value22 = rnd.Next(0, 25);
            int value33 = rnd.Next(0, 25);

            string value1 = film[value11].Substring(7, film[value11].Length - 8);
            string value2 = film[value22].Substring(7, film[value22].Length - 8);
            string value3 = film[value33].Substring(7, film[value33].Length - 8);

            string adress2 = $"https://moviehelperapi.azurewebsites.net/api/MovieSearcher/search_by_tittle_in_IMDb?tittle={value1}";
            string response2;
            HttpWebRequest httpWebRequest1 = (HttpWebRequest)WebRequest.Create(adress2);
            HttpWebResponse httpWebResponse1 = (HttpWebResponse)httpWebRequest1.GetResponse();
            using (StreamReader st = new StreamReader(httpWebResponse1.GetResponseStream()))
            {
                response2 = st.ReadToEnd();
            }
            film1 = JsonConvert.DeserializeObject<IMDb>(response2);

            string adress3 = $"https://moviehelperapi.azurewebsites.net/api/MovieSearcher/search_by_tittle_in_IMDb?tittle={value2}";
            string response3;
            HttpWebRequest httpWebRequest2 = (HttpWebRequest)WebRequest.Create(adress3);
            HttpWebResponse httpWebResponse2 = (HttpWebResponse)httpWebRequest2.GetResponse();
            using (StreamReader st = new StreamReader(httpWebResponse2.GetResponseStream()))
            {
                response3 = st.ReadToEnd();
            }
            film2 = JsonConvert.DeserializeObject<IMDb>(response3);

            string adress4 = $"https://moviehelperapi.azurewebsites.net/api/MovieSearcher/search_by_tittle_in_IMDb?tittle={value3}";
            string response4;
            HttpWebRequest httpWebRequest3 = (HttpWebRequest)WebRequest.Create(adress4);
            HttpWebResponse httpWebResponse3 = (HttpWebResponse)httpWebRequest3.GetResponse();
            using (StreamReader st = new StreamReader(httpWebResponse3.GetResponseStream()))
            {
                response4 = st.ReadToEnd();
            }
            film3 = JsonConvert.DeserializeObject<IMDb>(response4);
        }

        public static void film_from_genre(string genre, ref ranF film)
        {
            string adress = "https://moviehelperapi.azurewebsites.net/api/MovieSearcher/random_film";
            bool b = false;
            int n = 0;
            while (!b || n<10)
            {
                string response;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(adress);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (StreamReader st = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = st.ReadToEnd();
                }
                var res = JsonConvert.DeserializeObject<ranF>(response);
                for (int i = 0; i < res.data.movie.genres.Count; i++)
                {
                    if (genre == res.data.movie.genres[i])
                    {
                        b = true;
                        break;
                    }
                }
                film = res;
                n++;
            }
        }
        public static void film_from_years(int year1, int year2, ref ranF film)
        {
            string adress = "https://moviehelperapi.azurewebsites.net/api/MovieSearcher/random_film";
            int y = 0;
            bool b = false;
            while (!b)
            {
                string response;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(adress);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (StreamReader st = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = st.ReadToEnd();
                }
                var res = JsonConvert.DeserializeObject<ranF>(response);
                y = res.data.movie.year;
                if (year1 <= y && year2 >= y) b = true;
                film = res;
            }
        }
        public static void film_from_year(int year, ref ranF film)
        {
            string adress = "https://moviehelperapi.azurewebsites.net/api/MovieSearcher/random_film";
            int y=0;
            while (year!=y)
            {
                string response;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(adress);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (StreamReader st = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = st.ReadToEnd();
                }
                var res = JsonConvert.DeserializeObject<ranF>(response);
                y = res.data.movie.year;
                film = res;
            }
        }

        public static void int_favor(ref int n)
        {
            string adress = "https://moviehelperapi.azurewebsites.net/api/MovieSearcher/all_favorites_all";
            string response1;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(adress);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader st = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response1 = st.ReadToEnd();
            }
            var res = JsonConvert.DeserializeObject<List<film_post>>(response1);
            n = res.Count;
        }
        public static void GetFavorites(string IDD, HttpClient clientAPI, ref list_of_movies film2)
        {
            string adress = $"https://moviehelperapi.azurewebsites.net/api/MovieSearcher/all_favorites?userId={IDD}";
            string response1;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(adress);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader st = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response1 = st.ReadToEnd();
            }
            
            var res = JsonConvert.DeserializeObject<List<film_post>>(response1);
            List<int> ls = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            for (int i=0; i<res.Count; i++)
            {
                switch(res[i].genre)
                {
                    case "comedy": { ls[0]++; break; }
                    case "horror": { ls[1]++; break; }
                    case "action": { ls[2]++; break; }
                    case "romance": { ls[3]++; break; }
                    case "thriller": { ls[4]++; break; }
                    case "drama": { ls[5]++; break; }
                    case "mystery": { ls[6]++; break; }
                    case "crime": { ls[7]++; break; }
                    case "animation": { ls[8]++; break; }
                    case "adventure": { ls[9]++; break; }
                    case "fantasy": { ls[10]++; break; }
                    default: break;
                }
            }
            List<int> ls1 = ls;
            ls.Sort();
            int index=0;
            string genre_popular="";
            int max = ls[0];
            for (int i=0; i<ls1.Count; i++)
            {
                if (ls1[i] == max) index = i;
            }
            switch (index)
            {
                case 0: { genre_popular = "comedy"; break; }
                case 1: { genre_popular = "horror"; break; }
                case 2: { genre_popular = "action"; break; }
                case 3: { genre_popular = "romance"; break; }
                case 4: { genre_popular = "thriller"; break; }
                case 5: { genre_popular = "drama"; break; }
                case 6: { genre_popular = "mystery"; break; }
                case 7: { genre_popular = "crime"; break; }
                case 8: { genre_popular = "animation"; break; }
                case 9: { genre_popular = "adventure"; break; }
                case 10: { genre_popular = "fantasy"; break; }
            }
            Random rnd = new Random();
            int value = rnd.Next(2, 9);
            Console.WriteLine(genre_popular);
            string adress1 = "https://moviehelperapi.azurewebsites.net/api/MovieSearcher";
                adress1 += $"/choice_list_of_movies?count_of_films=30&minimum_rating=3&genre={genre_popular}";

                string response;

                HttpWebRequest httpWebRequest1 = (HttpWebRequest)WebRequest.Create(adress1);
                HttpWebResponse httpWebResponse1 = (HttpWebResponse)httpWebRequest1.GetResponse();
                using (StreamReader st1 = new StreamReader(httpWebResponse1.GetResponseStream()))
                {
                    response = st1.ReadToEnd();
                }
                film2 = JsonConvert.DeserializeObject<list_of_movies>(response);
                
            
        }

        public static async Task addfavorit(string IDD, string Per_ID, HttpClient clientAPI)
        {
            var result = await clientAPI.GetAsync($"/api/MovieSearcher/filmbyid?Id={IDD}");
            var content = result.Content.ReadAsStringAsync().Result;

            var film = JsonConvert.DeserializeObject<film_post>(content);
            var response = await clientAPI.DeleteAsync(new Uri($"https://moviehelperapi.azurewebsites.net/api/MovieSearcher?Id={IDD}"));
            response.EnsureSuccessStatusCode();
            film.UserId = Per_ID;
            film.Id = IDD;
            var json = JsonConvert.SerializeObject(film);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var post = await clientAPI.PostAsync("/api/MovieSearcher/add", data);
            post.EnsureSuccessStatusCode();
            
            var postcontent = post.Content.ReadAsStringAsync().Result;
            Console.WriteLine(postcontent);
        }
        public static async Task addAsync(film_post film, HttpClient clientAPI)
        {
            var json = JsonConvert.SerializeObject(film);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var post = await clientAPI.PostAsync("/api/MovieSearcher/add", data);
            post.EnsureSuccessStatusCode();

            var postcontent = post.Content.ReadAsStringAsync().Result;
        }
        public static void random_film(string _adress, ref ranF film)
        {
            _adress += "/random_film";
            string response;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_adress);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader st = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = st.ReadToEnd();
            }
            film = JsonConvert.DeserializeObject<ranF>(response);
        }

        public static void list_of_films(string _adress, int count, double mr, string genre, ref list_of_movies film1)
        {
            
            _adress += $"/choice_list_of_movies?count_of_films={count}&minimum_rating={mr}&genre={genre}";

            string response;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_adress);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader st = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = st.ReadToEnd();
            }
            film1 = JsonConvert.DeserializeObject<list_of_movies>(response);
        }
        public static void popular_list(string _adress, int count, double mr, ref list_of_movies film1, ref List<Movies> film2)
        {
            _adress += $"/popular_list?count_of_films={count}&minimum_rating={mr}";
            string response;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_adress);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader st = new StreamReader(httpWebResponse.GetResponseStream()))
            {
               response = st.ReadToEnd();
            }
            film1 = JsonConvert.DeserializeObject<list_of_movies>(response);
            int ind = film1.Data.Movies.Count;
            Console.WriteLine(ind);
            int k = 0;
            for (int i = 0; i < ind; i++)
            {
                if (film1.Data.Movies[i].Year > 2019)
                {
                    film2.Add(film1.Data.Movies[i]);
                }
                else k++;
            }
        }
    }
}

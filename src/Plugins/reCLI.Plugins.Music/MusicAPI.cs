using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace reCLI.Plugins.Music.Library
{
    // Data from Netease Muisc
    class MusicAPI
    {
        //From https://github.com/GEEKiDoS/NeteaseMuiscApi

        #region Data API

        public class MVResult
        {
            public string LoadingPic { get; set; }
            public string BufferPic { get; set; }
            public string LoadingPicFs { get; set; }
            public string BufferPicFs { get; set; }
            public bool Subed { get; set; }
            public Data Data { get; set; }
            public long Code { get; set; }
        }

        public class Data
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public long ArtistId { get; set; }
            public string ArtistName { get; set; }
            public string BriefDesc { get; set; }
            public string Desc { get; set; }
            public string Cover { get; set; }
            public long CoverId { get; set; }
            public long PlayCount { get; set; }
            public long SubCount { get; set; }
            public long ShareCount { get; set; }
            public long LikeCount { get; set; }
            public long CommentCount { get; set; }
            public long Duration { get; set; }
            public long NType { get; set; }
            public DateTime PublishTime { get; set; }
            public Brs Brs { get; set; }
            public Artist[] Artists { get; set; }
            public bool IsReward { get; set; }
            public string CommentThreadId { get; set; }
        }

        public class MVArtist
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }

        public class Brs
        {
            public string The480 { get; set; }
            public string The240 { get; set; }
            public string The720 { get; set; }
        }

        public class LyricResult
        {
            public bool Sgc { get; set; }
            public bool Sfy { get; set; }
            public bool Qfy { get; set; }
            public LyricUser TransUser { get; set; }
            public LyricUser LyricUser { get; set; }
            public Lrc Lrc { get; set; }
            public Klyric Klyric { get; set; }
            public Lrc Tlyric { get; set; }
            public long Code { get; set; }
        }

        public class Klyric
        {
            public long Version { get; set; }
        }

        public class Lrc
        {
            public long Version { get; set; }
            public string Lyric { get; set; }
        }

        public class LyricUser
        {
            public long Id { get; set; }
            public long Status { get; set; }
            public long Demand { get; set; }
            public long Userid { get; set; }
            public string Nickname { get; set; }
            public long Uptime { get; set; }
        }


        public class SongUrls
        {
            public Datum[] Data { get; set; }
            public long Code { get; set; }
        }

        public class PlayListResult
        {
            public Playlist Playlist { get; set; }
            public long Code { get; set; }
            public Privilege[] Privileges { get; set; }
        }

        public class Playlist
        {
            public object[] Subscribers { get; set; }
            public bool Subscribed { get; set; }
            public User Creator { get; set; }
            public Track[] Tracks { get; set; }
            public TrackId[] TrackIds { get; set; }
            public long CoverImgId { get; set; }
            public long CreateTime { get; set; }
            public long UpdateTime { get; set; }
            public bool NewImported { get; set; }
            public long Privacy { get; set; }
            public long SpecialType { get; set; }
            public string CommentThreadId { get; set; }
            public long TrackUpdateTime { get; set; }
            public long TrackCount { get; set; }
            public bool HighQuality { get; set; }
            public long SubscribedCount { get; set; }
            public long CloudTrackCount { get; set; }
            public string CoverImgUrl { get; set; }
            public long PlayCount { get; set; }
            public long AdType { get; set; }
            public long TrackNumberUpdateTime { get; set; }
            public object Description { get; set; }
            public bool Ordered { get; set; }
            public object[] Tags { get; set; }
            public long Status { get; set; }
            public long UserId { get; set; }
            public string Name { get; set; }
            public long Id { get; set; }
            public long ShareCount { get; set; }
            public string CoverImgIdStr { get; set; }
            public long CommentCount { get; set; }
        }

        public class User
        {
            public bool DefaultAvatar { get; set; }
            public long Province { get; set; }
            public long AuthStatus { get; set; }
            public bool Followed { get; set; }
            public string AvatarUrl { get; set; }
            public long AccountStatus { get; set; }
            public long Gender { get; set; }
            public long City { get; set; }
            public long Birthday { get; set; }
            public long UserId { get; set; }
            public long UserType { get; set; }
            public string Nickname { get; set; }
            public string Signature { get; set; }
            public string Description { get; set; }
            public string DetailDescription { get; set; }
            public long AvatarImgId { get; set; }
            public long BackgroundImgId { get; set; }
            public string BackgroundUrl { get; set; }
            public long Authority { get; set; }
            public bool Mutual { get; set; }
            public object ExpertTags { get; set; }
            public object Experts { get; set; }
            public long DjStatus { get; set; }
            public long VipType { get; set; }
            public object RemarkName { get; set; }
            public string BackgroundImgIdStr { get; set; }
            public string AvatarImgIdStr { get; set; }
        }

        public class Track
        {
            public string Name { get; set; }
            public long Id { get; set; }
            public long Pst { get; set; }
            public long T { get; set; }
            public Ar[] Ar { get; set; }
            public string[] Alia { get; set; }
            public double Pop { get; set; }
            public long St { get; set; }
            public string Rt { get; set; }
            public long Fee { get; set; }
            public long V { get; set; }
            public string Crbt { get; set; }
            public string Cf { get; set; }
            public Al Al { get; set; }
            public long Dt { get; set; }
            public H H { get; set; }
            public H M { get; set; }
            public H L { get; set; }
            public object A { get; set; }
            public string Cd { get; set; }
            public long No { get; set; }
            public object RtUrl { get; set; }
            public long Ftype { get; set; }
            public object[] RtUrls { get; set; }
            public long DjId { get; set; }
            public long Copyright { get; set; }
            public long SId { get; set; }
            public long Mst { get; set; }
            public long Cp { get; set; }
            public long Mv { get; set; }
            public long Rtype { get; set; }
            public object Rurl { get; set; }
            public long PublishTime { get; set; }
            public string[] Tns { get; set; }
        }

        public class TrackId
        {
            public long Id { get; set; }
            public long V { get; set; }
        }

        public class Datum
        {
            public long Id { get; set; }
            public string Url { get; set; }
            public long Br { get; set; }
            public long Size { get; set; }
            public string Md5 { get; set; }
            public long Code { get; set; }
            public long Expi { get; set; }
            public string Type { get; set; }
            public double Gain { get; set; }
            public long Fee { get; set; }
            public object Uf { get; set; }
            public long Payed { get; set; }
            public long Flag { get; set; }
            public bool CanExtend { get; set; }
        }

        public class SearchResult
        {
            public SResult Result { get; set; }
            public long Code { get; set; }
        }

        public class ArtistResult
        {
            public long Code { get; set; }
            public Artist Artist { get; set; }
            public bool More { get; set; }
            public List<HotSong> HotSongs { get; set; }
        }

        public class DetailResult
        {
            public Song[] Songs { get; set; }
            public Privilege[] Privileges { get; set; }
            public long Code { get; set; }
        }

        public class Artist
        {
            public long Img1V1Id { get; set; }
            public long TopicPerson { get; set; }
            public long PicId { get; set; }
            public object BriefDesc { get; set; }
            public long AlbumSize { get; set; }
            public string Img1V1Url { get; set; }
            public string PicUrl { get; set; }
            public List<string> Alias { get; set; }
            public string Trans { get; set; }
            public long MusicSize { get; set; }
            public string Name { get; set; }
            public long Id { get; set; }
            public long PublishTime { get; set; }
            public long MvSize { get; set; }
            public bool Followed { get; set; }
        }

        public class AlbumResult
        {
            public Song[] Songs { get; set; }
            public long Code { get; set; }
            public Album Album { get; set; }
        }

        public class Album
        {
            public object[] Songs { get; set; }
            public bool Paid { get; set; }
            public bool OnSale { get; set; }
            public long PicId { get; set; }
            public object[] Alias { get; set; }
            public string CommentThreadId { get; set; }
            public long PublishTime { get; set; }
            public string Company { get; set; }
            public long CopyrightId { get; set; }
            public string PicUrl { get; set; }
            public Artist Artist { get; set; }
            public object BriefDesc { get; set; }
            public string Tags { get; set; }
            public Artist[] Artists { get; set; }
            public long Status { get; set; }
            public object Description { get; set; }
            public object SubType { get; set; }
            public string BlurPicUrl { get; set; }
            public long CompanyId { get; set; }
            public long Pic { get; set; }
            public string Name { get; set; }
            public long Id { get; set; }
            public string Type { get; set; }
            public long Size { get; set; }
            public string PicIdStr { get; set; }
            public Info Info { get; set; }
        }

        public class Info
        {
            public CommentThread CommentThread { get; set; }
            public object LatestLikedUsers { get; set; }
            public bool Liked { get; set; }
            public object Comments { get; set; }
            public long ResourceType { get; set; }
            public long ResourceId { get; set; }
            public long CommentCount { get; set; }
            public long LikedCount { get; set; }
            public long ShareCount { get; set; }
            public string ThreadId { get; set; }
        }

        public class CommentThread
        {
            public string Id { get; set; }
            public ResourceInfo ResourceInfo { get; set; }
            public long ResourceType { get; set; }
            public long CommentCount { get; set; }
            public long LikedCount { get; set; }
            public long ShareCount { get; set; }
            public long HotCount { get; set; }
            public object LatestLikedUsers { get; set; }
            public long ResourceId { get; set; }
            public long ResourceOwnerId { get; set; }
            public string ResourceTitle { get; set; }
        }

        public class ResourceInfo
        {
            public long Id { get; set; }
            public long UserId { get; set; }
            public string Name { get; set; }
            public object ImgUrl { get; set; }
            public object Creator { get; set; }
        }


        public class HotSong
        {
            public List<object> RtUrls { get; set; }
            public List<Ar> Ar { get; set; }
            public Al Al { get; set; }
            public long St { get; set; }
            public long Fee { get; set; }
            public long Ftype { get; set; }
            public long Rtype { get; set; }
            public object Rurl { get; set; }
            public long T { get; set; }
            public string Cd { get; set; }
            public long No { get; set; }
            public long V { get; set; }
            public object A { get; set; }
            public H M { get; set; }
            public long DjId { get; set; }
            public object Crbt { get; set; }
            public object RtUrl { get; set; }
            public List<object> Alia { get; set; }
            public long Pop { get; set; }
            public string Rt { get; set; }
            public long Mst { get; set; }
            public long Cp { get; set; }
            public string Cf { get; set; }
            public long Dt { get; set; }
            public long Pst { get; set; }
            public H H { get; set; }
            public H L { get; set; }
            public long Mv { get; set; }
            public string Name { get; set; }
            public long Id { get; set; }
            public Privilege Privilege { get; set; }
        }

        public class SResult
        {
            public List<Song> Songs { get; set; }
            public long SongCount { get; set; }
        }

        public class Song
        {
            public string Name { get; set; }
            public long Id { get; set; }
            public long Pst { get; set; }
            public long T { get; set; }
            public List<Ar> Ar { get; set; }
            public string ShortAr { get; set; }
            public List<object> Alia { get; set; }
            public long Pop { get; set; }
            public long St { get; set; }
            public string Rt { get; set; }
            public long Fee { get; set; }
            public long V { get; set; }
            public object Crbt { get; set; }
            public string Cf { get; set; }
            public Al Al { get; set; }
            public long Dt { get; set; }
            public H H { get; set; }
            public H M { get; set; }
            public H L { get; set; }
            public object A { get; set; }
            public string Cd { get; set; }
            public long No { get; set; }
            public object RtUrl { get; set; }
            public long Ftype { get; set; }
            public List<object> RtUrls { get; set; }
            public object Rurl { get; set; }
            public long Rtype { get; set; }
            public long Mst { get; set; }
            public long Cp { get; set; }
            public long Mv { get; set; }
            public long PublishTime { get; set; }
            public Privilege Privilege { get; set; }
        }

        public class Al
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string PicUrl { get; set; }
            public List<object> Tns { get; set; }
            public long Pic { get; set; }
        }

        public class Ar
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public List<object> Tns { get; set; }
            public List<object> Alias { get; set; }
        }

        public class H
        {
            public long Br { get; set; }
            public long Fid { get; set; }
            public long Size { get; set; }
            public double Vd { get; set; }
        }

        public class Privilege
        {
            public long Id { get; set; }
            public long Fee { get; set; }
            public long Payed { get; set; }
            public long St { get; set; }
            public long Pl { get; set; }
            public long Dl { get; set; }
            public long Sp { get; set; }
            public long Cp { get; set; }
            public long Subp { get; set; }
            public bool Cs { get; set; }
            public long Maxbr { get; set; }
            public long Fl { get; set; }
            public bool Toast { get; set; }
            public long Flag { get; set; }
        }

        #endregion

        public class NeteaseMusicAPI
        {
            // General
            private string _MODULUS = "00e0b509f6259df8642dbc35662901477df22677ec152b5ff68ace615bb7b725152b3ab17a876aea8a5aa76d2e417629ec4ee341f56135fccf695280104e0312ecbda92557c93870114af6c9d05c4f7f0c3685b7a46bee255932575cce10b424d813cfe4875d3e82047b97ddef52741d546b8e289dc6935b3ece0462db0a22b8e7";
            private string _NONCE = "0CoJUm6Qyw8W8jud";
            private string _PUBKEY = "010001";
            private string _VI = "0102030405060708";
            private string _USERAGENT = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
            private string _COOKIE = "os=pc;osver=Microsoft-Windows-10-Professional-build-16299.125-64bit;appver=2.0.3.131777;channel=netease;__remember_me=true";
            private string _REFERER = "http://music.163.com/";
            // use keygen in c#
            private string _secretKey;
            private string _encSecKey;

            public NeteaseMusicAPI()
            {
                _secretKey = CreateSecretKey(16);
                _encSecKey = RSAEncode(_secretKey);
            }

            private string CreateSecretKey(int length)
            {
                var str = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var r = "";
                var rnd = new Random();
                for (int i = 0; i < length; i++)
                {
                    r += str[rnd.Next(0, str.Length)];
                }
                return r;
            }

            private Dictionary<string, string> Prepare(string raw)
            {
#pragma warning disable IDE0028 // 简化集合初始化
                Dictionary<string, string> data = new Dictionary<string, string>();
#pragma warning restore IDE0028 // 简化集合初始化
                data["params"] = AESEncode(raw, _NONCE);
                data["params"] = AESEncode(data["params"], _secretKey);
                data["encSecKey"] = _encSecKey;

                return data;
            }

            // encrypt mod
            private string RSAEncode(string text)
            {
                string srtext = new string(text.Reverse().ToArray()); ;
                var a = BCHexDec(BitConverter.ToString(Encoding.Default.GetBytes(srtext)).Replace("-", ""));
                var b = BCHexDec(_PUBKEY);
                var c = BCHexDec(_MODULUS);
                var key = BigInteger.ModPow(a, b, c).ToString("x");
                key = key.PadLeft(256, '0');
                if (key.Length > 256)
                    return key.Substring(key.Length - 256, 256);
                else
                    return key;
            }

            private BigInteger BCHexDec(string hex)
            {
                BigInteger dec = new BigInteger(0);
                int len = hex.Length;
                for (int i = 0; i < len; i++)
                {
                    dec += BigInteger.Multiply(new BigInteger(Convert.ToInt32(hex[i].ToString(), 16)), BigInteger.Pow(new BigInteger(16), len - i - 1));
                }
                return dec;
            }

            private string AESEncode(string secretData, string secret = "TA3YiYCfY2dDJQgg")
            {
                byte[] encrypted;
                byte[] IV = Encoding.UTF8.GetBytes(_VI);

                using (var aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(secret);
                    aes.IV = IV;
                    aes.Mode = CipherMode.CBC;
                    using (var encryptor = aes.CreateEncryptor())
                    {
                        using (var stream = new MemoryStream())
                        {
                            using (var cstream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                            {
                                using (var sw = new StreamWriter(cstream))
                                {
                                    sw.Write(secretData);
                                }
                                encrypted = stream.ToArray();
                            }
                        }
                    }
                }
                return Convert.ToBase64String(encrypted);
            }

            // fake curl
            private string CURL(string url, Dictionary<string, string> parms, string method = "POST")
            {
                string result;
                using (var wc = new WebClient())
                {
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                    wc.Headers.Add(HttpRequestHeader.Referer, _REFERER);
                    wc.Headers.Add(HttpRequestHeader.UserAgent, _USERAGENT);
                    wc.Headers.Add(HttpRequestHeader.Cookie, _COOKIE);
                    var reqparm = new System.Collections.Specialized.NameValueCollection();
                    foreach (var keyPair in parms)
                    {
                        reqparm.Add(keyPair.Key, keyPair.Value);
                    }

                    byte[] responsebytes = wc.UploadValues(url, method, reqparm);
                    result = Encoding.UTF8.GetString(responsebytes);
                }
                return result;
            }

            // api start
            private class SearchJson
            {
                public string s;
                public int type;
                public int limit;
                public string total = "true";
                public int offset;
                public string csrf_token = "";
            }

            public enum SearchType
            {
                Song = 1,
                Album = 10,
                Artist = 100,
                PlayList = 1000,
                User = 1002,
                Radio = 1009,
            }

            public SearchResult Search(string keyword, int limit = 30, int offset = 0, SearchType type = SearchType.Song)
            {
                var url = "http://music.163.com/weapi/cloudsearch/get/web";
                var data = new SearchJson
                {
                    s = keyword,
                    type = (int)type,
                    limit = limit,
                    offset = offset,
                };

                string raw = CURL(url, Prepare(JsonConvert.SerializeObject(data)));

                var DeserialedObj = JsonConvert.DeserializeObject<SearchResult>(raw);

                return DeserialedObj;
            }


            public ArtistResult Artist(long artist_id)
            {
                var url = "http://music.163.com/weapi/v1/artist/" + artist_id.ToString() + "?csrf_token=";
                var data = new Dictionary<string, string>
            {
                {"csrf_token",""}
            };
                var raw = CURL(url, Prepare(JsonConvert.SerializeObject(data)));

                var deserialedObj = JsonConvert.DeserializeObject<ArtistResult>(raw);
                return deserialedObj;
            }

            public AlbumResult Album(long album_id)
            {
                string url = "http://music.163.com/weapi/v1/album/" + album_id.ToString() + "?csrf_token=";
                var data = new Dictionary<string, string> {
                { "csrf_token","" },
            };
                string raw = CURL(url, Prepare(JsonConvert.SerializeObject(data)));
                var deserialedObj = JsonConvert.DeserializeObject<AlbumResult>(raw);
                return deserialedObj;
            }

            public DetailResult Detail(long song_id)
            {
                string url = "http://music.163.com/weapi/v3/song/detail?csrf_token=";
                var data = new Dictionary<string, string> {
                { "c",
                    "[" + JsonConvert.SerializeObject(new Dictionary<string, string> { //神tm 加密的json里套json mdzz (说不定一次可以查多首歌?)
                        { "id", song_id.ToString() }
                    }) + "]"
                },
                {"csrf_token",""},
            };
                string raw = CURL(url, Prepare(JsonConvert.SerializeObject(data)));

                var deserialedObj = JsonConvert.DeserializeObject<DetailResult>(raw);
                return deserialedObj;
            }

            private class GetSongUrlJson
            {
                public long[] ids;
                public long br;
                public string csrf_token = "";
            }

            public SongUrls GetSongsUrl(long[] song_id, long bitrate = 999000)
            {
                string url = "http://music.163.com/weapi/song/enhance/player/url?csrf_token=";
                
                var data = new GetSongUrlJson
                {
                    ids = song_id,
                    br = bitrate
                };

                string raw = CURL(url, Prepare(JsonConvert.SerializeObject(data)));

                var deserialedObj = JsonConvert.DeserializeObject<SongUrls>(raw);
                return deserialedObj;
            }

            /*public SongUrls GetSongsUrl(long id)
            {
                string url = $"http://musicapi.leanapp.cn/music/url?id={id}";

                string raw = CURL(url,new Dictionary<string, string>(),"GET");

                var deserialedObj = JsonConvert.DeserializeObject<SongUrls>(raw);
                return deserialedObj;
            }*/

            public PlayListResult Playlist(long playlist_id)
            {
                string url = "http://music.163.com/weapi/v3/playlist/detail?csrf_token=";
                var data = new Dictionary<string, string> {
                { "id",playlist_id.ToString() },
                { "n" , "1000" },
                { "csrf_token" , "" },
            };
                string raw = CURL(url, Prepare(JsonConvert.SerializeObject(data)));

                var deserialedObj = JsonConvert.DeserializeObject<PlayListResult>(raw);
                return deserialedObj;
            }

            public LyricResult Lyric(long song_id)
            {
                string url = "http://music.163.com/weapi/song/lyric?csrf_token=";
                var data = new Dictionary<string, string> {
                { "id",song_id.ToString()},
                { "os","pc" },
                { "lv","-1" },
                { "kv","-1" },
                { "tv","-1" },
                { "csrf_token","" }
            };

                string raw = CURL(url, Prepare(JsonConvert.SerializeObject(data)));
                var deserialedObj = JsonConvert.DeserializeObject<LyricResult>(raw);
                return deserialedObj;
            }

            public MVResult MV(int mv_id)
            {
                string url = "http://music.163.com/weapi/mv/detail?csrf_token=";
                var data = new Dictionary<string, string> {
                { "id",mv_id.ToString() },
                { "csrf_token","" },
            };
                string raw = CURL(url, Prepare(JsonConvert.SerializeObject(data)));
                var deserialedObj = JsonConvert.DeserializeObject<MVResult>(
                    raw.Replace("\"720\"", "\"the720\"")
                       .Replace("\"480\"", "\"the480\"")
                       .Replace("\"240\"", "\"the240\"")); //不能解析数字key的解决方案
                return deserialedObj;
            }

            //static url encrypt, use for pic

            public string Id2Url(int id)
            {
                byte[] magic = Encoding.ASCII.GetBytes("3go8&8*3*3h0k(2)2");
                byte[] song_id = Encoding.ASCII.GetBytes(id.ToString());

                for (int i = 0; i < song_id.Length; i++)
                    song_id[i] = Convert.ToByte(song_id[i] ^ magic[i % magic.Length]);

                string result;

                using (var md5 = MD5.Create())
                {
                    md5.ComputeHash(song_id);
                    result = Convert.ToBase64String(md5.Hash);
                }

                result = result.Replace("/", "_");
                result = result.Replace("+", "-");
                return result;
            }
        }
    }
}

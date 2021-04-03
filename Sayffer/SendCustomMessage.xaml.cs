using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Firebase.Auth;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Sayffer.Helper;
using Xamarin.Forms;
using Sayffer.Model;

namespace Sayffer
{
    public partial class SendCustomMessage : ContentPage
    {
        public SendCustomMessage()
        {
            InitializeComponent();
            GetProfilInformationAndRefreshToken();

        }
        public string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

        async void GetProfilInformationAndRefreshToken()
        {
            try
            {

                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));

                var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
                var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
                Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
                string UsersEmailToDisplay = savedfirebaseauth.User.Email;
                Person person = await FirebaseHelper.GetPerson();
                string cityschool = (person.City.ToLower() + person.School.ToLower()).Replace(" ", "");
                var listofclasscodes = new List<string>()
            {
                "All people in school",
                "All teachers",
                "All parents",
                "All administators"
            };
                var personclasscode = await FirebaseHelper.GetPersonsClassCodes();
                ClassCodeSend.ItemsSource = personclasscode;
                var allclasscodes = await FirebaseHelper.GetAllClassCodes(cityschool);
                foreach (var classcode in allclasscodes)
                {
                    string classcodeforuse = classcode.ToString();
                    if (listofclasscodes.Contains(classcodeforuse.ToLower()))
                    {
                        listofclasscodes = listofclasscodes;
                    }
                    else
                    {
                        if (classcodeforuse.ToString() == "emptyatpresent")
                        {
                            listofclasscodes = listofclasscodes;
                        }
                        else
                        {
                            if (classcodeforuse != "emptyatpresent" && classcodeforuse != "EmptyAtPresent")
                            {
                                listofclasscodes.Add(classcodeforuse.ToLower());

                            }
                        }

                    }


                }

                if (person.Role != "Nurse" && person.Role != "Admin")
                {
                    SendTo.SetValue(IsVisibleProperty, false);

                }
                else
                {
                    SendTo.ItemsSource = listofclasscodes;

                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                await App.Current.MainPage.DisplayAlert("Oops!", "Token Expired", "OK");
            }
        }

        async void Send_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {

                if ((MessageText.Text).Length >= 101)
                {
                    await App.Current.MainPage.DisplayAlert("Oops!", "Message exceeds character limit", "OK");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Done!", "Message sent", "OK");
                    var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));

                    var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
                    var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
                    Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
                    string UsersEmailToDisplay = savedfirebaseauth.User.Email;
                    if (SendTo.IsVisible)
                    {
                        if (SendTo.SelectedItem.ToString() == "All people in school" || SendTo.SelectedItem.ToString() == "All teachers" || SendTo.SelectedItem.ToString() == "All parents" || SendTo.SelectedItem.ToString() == "All administators")
                        {
                            if (SendTo.SelectedItem.ToString() == "All people in school")
                            {
                                string CitySchool = (await FirebaseHelper.GetPersonCity() + await FirebaseHelper.GetPersonSchool()).Replace(" ", "").ToLower();
                                /*if (Device.RuntimePlatform == "Android")
                                {
                                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                                    tRequest.Method = "post";
                                    string serverkey = "AAAAgDqIECw:APA91bFMNd3twf8jomqZyXNkY7zz8CvBX5ZlfknEEVoa6RtjkfKhMFhBy4onehiMuYVCTR91T5uHxJ7ie_zBmacMJZlVr-GEzxK3zN0s20HNjBbeTj2JzpUNdADRGv9VEUl4SPm5YoWT";
                                    string senderid = "550737809452";
                                    tRequest.Headers.Add(string.Format("Authorization: key={0}", serverkey));
                                    tRequest.Headers.Add(string.Format("Sender: id={0}", senderid));
                                    tRequest.ContentType = "application/json";
                                    var message = new
                                    {
                                        to = "/topics/" + CitySchool,
                                        priority = "high",
                                        content_available = true,
                                        notification = new
                                        {
                                            title = "New Message Notification from " + person.Name,
                                            body = MessageText.Text

                                        },
                                    };
                                    string postbody = JsonConvert.SerializeObject(message).ToString();
                                    Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                                    tRequest.ContentLength = byteArray.Length;
                                    using (Stream dataStream = tRequest.GetRequestStream())
                                    {
                                        dataStream.Write(byteArray, 0, byteArray.Length);
                                        using (WebResponse tResponse = tRequest.GetResponse())
                                        {
                                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                                            {
                                                if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                                    {
                                                        String sResponseFromServer = tReader.ReadToEnd();
                                                        //result.Response = sResponseFromServer;
                                                    }
                                            }
                                        }
                                    }
                                }*/
                            }
                            if (SendTo.SelectedItem.ToString() == "All teachers")
                            {

                            }

                        }
                        else
                        {
                            /*if (Device.RuntimePlatform == "Android")
                            {
                                string CitySchool = (person.City.ToLower() + person.School.ToLower()).Replace(" ", "");
                                string sendto = (CitySchool + SendTo.SelectedItem.ToString()).ToLower().Replace(" ","");
                                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                                tRequest.Method = "post";
                                string serverkey = "AAAAgDqIECw:APA91bFMNd3twf8jomqZyXNkY7zz8CvBX5ZlfknEEVoa6RtjkfKhMFhBy4onehiMuYVCTR91T5uHxJ7ie_zBmacMJZlVr-GEzxK3zN0s20HNjBbeTj2JzpUNdADRGv9VEUl4SPm5YoWT";
                                string senderid = "550737809452";
                                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverkey));
                                tRequest.Headers.Add(string.Format("Sender: id={0}", senderid));
                                tRequest.ContentType = "application/json";
                                var message = new
                                {
                                    to = "/topics/" + sendto,
                                    priority = "high",
                                    content_available = true,
                                    notification = new
                                    {
                                        title = "New Message Notification from " + person.Name,
                                        body = MessageText.Text

                                    },
                                };
                                string postbody = JsonConvert.SerializeObject(message).ToString();
                                Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                                tRequest.ContentLength = byteArray.Length;
                                using (Stream dataStream = tRequest.GetRequestStream())
                                {
                                    dataStream.Write(byteArray, 0, byteArray.Length);
                                    using (WebResponse tResponse = tRequest.GetResponse())
                                    {
                                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                                        {
                                            if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                                {
                                                    String sResponseFromServer = tReader.ReadToEnd();
                                                    //result.Response = sResponseFromServer;
                                                }
                                        }
                                    }
                                }
                            }*/

                        }
                        if (SendTo.SelectedItem.ToString() == "All teachers")
                        {
                            /*if (Device.RuntimePlatform == "Android")
                            {
                                string CitySchool = (person.City.ToLower() + person.School.ToLower()).Replace(" ", "");
                                string sendto = CitySchool + "teachers";
                                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                                tRequest.Method = "post";
                                string serverkey = "AAAAgDqIECw:APA91bFMNd3twf8jomqZyXNkY7zz8CvBX5ZlfknEEVoa6RtjkfKhMFhBy4onehiMuYVCTR91T5uHxJ7ie_zBmacMJZlVr-GEzxK3zN0s20HNjBbeTj2JzpUNdADRGv9VEUl4SPm5YoWT";
                                string senderid = "550737809452";
                                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverkey));
                                tRequest.Headers.Add(string.Format("Sender: id={0}", senderid));
                                tRequest.ContentType = "application/json";
                                var message = new
                                {
                                    to = "/topics/" + sendto,
                                    priority = "high",
                                    content_available = true,
                                    notification = new
                                    {
                                        title = "New Message Notification from " + person.Name,
                                        body = MessageText.Text

                                    },
                                };
                                string postbody = JsonConvert.SerializeObject(message).ToString();
                                Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                                tRequest.ContentLength = byteArray.Length;
                                using (Stream dataStream = tRequest.GetRequestStream())
                                {
                                    dataStream.Write(byteArray, 0, byteArray.Length);
                                    using (WebResponse tResponse = tRequest.GetResponse())
                                    {
                                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                                        {
                                            if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                                {
                                                    String sResponseFromServer = tReader.ReadToEnd();
                                                    //result.Response = sResponseFromServer;
                                                }
                                        }
                                    }
                                }
                            }
                        }*/
                        }
                        else
                        {
                            /*if (Device.RuntimePlatform == "Android")
                            {
                                string CitySchool = (person.City.ToLower() + person.School.ToLower()).Replace(" ", "");
                                string sendto = (CitySchool + person.ClassCode).ToLower().Replace(" ", "");
                                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                                tRequest.Method = "post";
                                string serverkey = "AAAAgDqIECw:APA91bFMNd3twf8jomqZyXNkY7zz8CvBX5ZlfknEEVoa6RtjkfKhMFhBy4onehiMuYVCTR91T5uHxJ7ie_zBmacMJZlVr-GEzxK3zN0s20HNjBbeTj2JzpUNdADRGv9VEUl4SPm5YoWT";
                                string senderid = "550737809452";
                                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverkey));
                                tRequest.Headers.Add(string.Format("Sender: id={0}", senderid));
                                tRequest.ContentType = "application/json";
                                var message = new
                                {
                                    to = "/topics/" + sendto,
                                    priority = "high",
                                    content_available = true,
                                    notification = new
                                    {
                                        title = "New Message Notification from " + person.Name,
                                        body = MessageText.Text
                                    },
                                };
                                string postbody = JsonConvert.SerializeObject(message).ToString();
                                Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                                tRequest.ContentLength = byteArray.Length;
                                using (Stream dataStream = tRequest.GetRequestStream())
                                {
                                    dataStream.Write(byteArray, 0, byteArray.Length);
                                    using (WebResponse tResponse = tRequest.GetResponse())
                                    {
                                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                                        {
                                            if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                                {
                                                    String sResponseFromServer = tReader.ReadToEnd();
                                                    //result.Response = sResponseFromServer;
                                                }
                                        }
                                    }
                                }
                            }
                        }*/


                        }

                    }
                    else
                    {
                        string school = await FirebaseHelper.GetPersonSchool();
                        string city = await FirebaseHelper.GetPersonCity();
                        await FirebaseHelper.SendMessageToClass(school, city, MessageText.Text, ClassCodeSend.SelectedItem.ToString(), await FirebaseHelper.GetPersonName((city + school).ToLower().Replace(" ", "")), ClassCodeSend.SelectedItem.ToString());
                    }

                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                await App.Current.MainPage.DisplayAlert("Oops!", "Token Expired", "OK");
            }

        }
        async void BackToDashboard_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new DashboardPage());
        }
    }
}

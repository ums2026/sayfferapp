using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using Sayffer.Model;
using Xamarin.Essentials;

namespace Sayffer.Helper
{
    public class FirebaseHelper
    {

        public static FirebaseClient firebase = new FirebaseClient("https://xamarinformsfirebase2-92714.firebaseio.com/");
        public static string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

        public static string url = "https://xamarinformsfirebase2-92714.firebaseio.com/";

        //Get class allergies info from the classcode. need to create method for finding the codes
        public static async Task<List<Person>> GetClassAllergiesInfoParents(string city, string school, string classcode)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            string CitySchool = city.ToLower() + school.ToLower();

            return (await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("ParentInfo")
                  .Child(classcode)
                  .Child("FoodAllergies")
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      FoodAllergies = item.Object.FoodAllergies.ToString(),
                      AllergyDetails = "Allergy severity: " + item.Object.AllergyDetails.ToString(),
                      ClassCode = classcode

                  }).ToList();

        }
        public static async Task<List<Person>> GetClassAllergiesInfo(string city, string school, string classcode)
        {

            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            try

            {
                string Email = UsersEmailToDisplay;
                Firebase.Auth.User user = savedfirebaseauth.User;

                var firebaseClient = new FirebaseClient(
                                url,
                                new FirebaseOptions
                                {
                                    AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                                });
                string CitySchool = city.ToLower() + school.ToLower();

                return (await firebaseClient
                      .Child(CitySchool.Replace(" ", ""))
                      .Child("TeacherInfo")
                      .Child(classcode.ToLower())
                      .Child(user.LocalId)
                      .Child("FoodAllergies")
                      .OnceAsync<Person>())
                      .Select(item => new Person
                      {
                          FoodAllergies = item.Object.FoodAllergies.ToString(),
                          AllergyDetails = "Allergy severity: " + item.Object.AllergyDetails.ToString(),
                          ClassCode = item.Object.ClassCode,
                          NewUserEmail = item.Object.Name + " (" + item.Object.NewUserEmail + ")"

                      })
                      .Where(a => a.ClassCode == classcode)
                      .ToList();
            }
            catch (Exception x)
            {
                Person person = new Person { FoodAllergies = "None entered", AllergyDetails = " ", ClassCode = " ", NewUserEmail = " " };
                var list = new List<Person>();
                list.Add(person);
                return list;

            }


        }
        //Gets detailed info, need to store like this
        public static async Task<List<Person>> GetClassAllergiesInfoNurses(string city, string school, string classcode)
        {
            try
            {
                string CitySchool = city.ToLower() + school.ToLower();
                string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
                var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
                var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
                Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
                string UsersEmailToDisplay = savedfirebaseauth.User.Email;
                string Email = UsersEmailToDisplay;
                Firebase.Auth.User user = savedfirebaseauth.User;

                var firebaseClient = new FirebaseClient(
                                url,
                                new FirebaseOptions
                                {
                                    AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                                });
                return (await firebaseClient
                      .Child(CitySchool.Replace(" ", ""))
                      .Child("NurseInfo")
                      .Child(user.LocalId)
                      .Child("FoodAllergies")
                      .OnceAsync<Person>())
                      .Select(item => new Person
                      {
                          FoodAllergies = item.Object.FoodAllergies.ToString() + " - " + item.Object.AllergyDetails.ToString(),
                          NewUserEmail = "Email: " + item.Object.NewUserEmail,
                          Name = "Student Name: " + item.Object.Name,
                          ClassCode = item.Object.ClassCode,
                          ParentName = "Parent Name: " + item.Object.ParentName

                      })
                      .Where(a => a.ClassCode.ToString() == classcode)
                      .Where(a => a.ParentName != "Parent Name: Teacher")
                      .ToList();
            }
            catch (Exception x)
            {
                Person person = new Person { FoodAllergies = "None entered", AllergyDetails = " ", ClassCode = " ", NewUserEmail = " " };
                var list = new List<Person>();
                list.Add(person);
                return list;

            }


        }
        //For the nurses picker for filtering out
        public static async Task<List<string>> GetAllClassCodes(string cityschool)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });

            var classcodes = (await firebaseClient
            .Child(cityschool)
            .Child("Classes")
            .Child(user.LocalId)
            .OnceAsync<Person>())
            .Select(item => new Person
            {
                ClassCode = item.Object.ClassCode.ToString().ToLower(),

            })

            .ToList();

            var list = new List<string>()
            {

            };
            foreach (var code in classcodes)
            {
                list.Add(code.ClassCode);
            }
            return list;

        }


        //Get Medical Allergies Info for Teachers when given the classcode. This should be displayed for the nurse 
        public static async Task<List<Person>> GetMedicalAllergyInfoByClassTeachers(string city, string school, string classcode)
        {
            string CitySchool = city.ToLower() + school.ToLower();
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            return (await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("NurseInfo")
                  .Child(user.LocalId)
                  .Child("MedicalAllergies")
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      MedicalAllergies = item.Object.MedicalAllergies + " (" + item.Object.Role.ToString() + ")",
                      NewUserEmail = item.Object.NewUserEmail.ToString() + ", " + item.Object.TeacherName.ToString(),
                      ClassCode = item.Object.ClassCode.ToString().ToLower(),
                      Role = item.Object.Role,
                      Name = item.Object.Name

                  }).Where(a => a.ClassCode == classcode.ToLower()).Where(a => a.Role.ToString().ToLower() == "teacher")

                  .ToList();

        }
        //Gets medical allergies by classcode for students, store "Name" as student name and "ParentName" as parent name
        public static async Task<List<Person>> GetMedicalAllergyInfoByClassStudents(string city, string school, string classcode)
        {
            string CitySchool = city.ToLower() + school.ToLower();
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            return (await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("NurseInfo")
                  .Child(user.LocalId)
                  .Child("MedicalAllergies")
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      MedicalAllergies = item.Object.MedicalAllergies,
                      NewUserEmail = item.Object.NewUserEmail.ToString(),
                      StudentName = "Student's Name: " + item.Object.Name.ToString(),
                      ParentName = "Parent's Name: " + item.Object.ParentName.ToString(),
                      ClassCode = item.Object.ClassCode.ToString().ToLower()

                  }).Where(a => (a.ClassCode == classcode.ToLower()))

                  .ToList();

        }
        //Gets virus case information by classcode
        public static async Task<List<Person>> GetVirusCaseInfoByClass(string city, string school, string classcode)
        {
            string CitySchool = city.ToLower() + school.ToLower();
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            return (await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("NurseInfo")
                  .Child(user.LocalId)
                  .Child("VirusCases")
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      HealthInfoVirusCase = item.Object.HealthInfoVirusCase.ToString(),
                      Time = item.Object.Time.ToString(),
                      Name = item.Object.Name.ToString(),
                      NewUserEmail = item.Object.NewUserEmail.ToString() + ", " + item.Object.Role.ToString(),
                      ClassCode = item.Object.ClassCode,

                  }).Where(a => a.ClassCode.ToLower() == classcode.ToLower())
                  .ToList();


        }

        //Get covid numbers from database, use for the covid data at a glance
        public static async Task<Person> GetCovidNumbersSchool(string city, string school)
        {
            string CitySchool = city.ToLower() + school.ToLower();
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            Person numbers = await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("COVID Numbers")
                  .OnceSingleAsync<Person>();
            return numbers;


        }
        //Get number of viral flu numbers by school
        public static async Task<Person> GetFluNumbersSchool(string city, string school)
        {
            string CitySchool = city.ToLower() + school.ToLower();
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            Person numbers = await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("Viral Flu Numbers")
                  .OnceSingleAsync<Person>();
            return numbers;

        }


        /*public static async Task<List<Person>> GetUnreadNotifications(string city, string school, string email)
        {
            string CitySchool = city.ToLower() + school.ToLower();

            return (await firebase
                  
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("Notifications")
                  .Child(email.ToLower().Replace(".",","))
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      Notification = item.Object.Notification.ToString(),
                      Time = item.Object.Time.ToString(),
                      NotificationStatus = item.Object.NotificationStatus.ToString()

                  })
                  .Where(a => a.NotificationStatus == "Unread")
                  .ToList();

        }*/
        public static async Task<List<Person>> GetUnreadNotifications(string city, string school, string email)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });

            string CitySchool = city.ToLower() + school.ToLower();
            return (await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("Notifications")
                  .Child(user.LocalId)
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      Notification = item.Object.Notification.ToString(),
                      Time = item.Object.Time.ToString(),
                      NotificationStatus = item.Object.NotificationStatus.ToString()

                  })
                  .Where(a => a.NotificationStatus == "Unread")
                  .ToList();

        }
        /*public static async Task<List<Person>> GetAllNotifications(string city, string school, string email)
        {

            string CitySchool = city.ToLower() + school.ToLower();
            var unread = (await firebase
                  
                  .Child(CitySchool.Replace("" + " ", ""))
                  .Child("Notifications")
                  .Child(email.ToLower().Replace(".", ","))
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      Notification = "(New) " + item.Object.Notification.ToString(),
                      Time = item.Object.Time.ToString(),
                      NotificationStatus = item.Object.NotificationStatus.ToString()

                  })
                  .Where(a => a.NotificationStatus == "Unread")
                  .ToList();
            var read = (await firebase
                  
                  .Child(CitySchool.Replace("" + " ", ""))
                  .Child("Notifications")
                  .Child(email.ToLower().Replace(".", ","))
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      Notification = item.Object.Notification.ToString(),
                      Time = item.Object.Time.ToString(),
                      NotificationStatus = item.Object.NotificationStatus.ToString()

                  })
                  .Where(a => a.NotificationStatus == "Read")
                  .ToList();
            if (unread.Count() != 0)
            {
                foreach (var element in unread)
                {
                    read.Insert(0, element);
                }

            }

            return read
             .OrderByDescending(d => Convert.ToDateTime(d.Time).Year)
            .ThenByDescending(d => Convert.ToDateTime(d.Time).Date).ToList();
            
        }*/
        public static async Task<List<Person>> GetAllNotifications(string city, string school, string email)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });

            string CitySchool = city.ToLower() + school.ToLower();
            var unread = (await firebaseClient

                  .Child(CitySchool.Replace(" ", ""))
                  .Child("Notifications")
                  .Child(user.LocalId)
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      Notification = "(New) " + item.Object.Notification.ToString(),
                      Time = item.Object.Time.ToString(),
                      NotificationStatus = item.Object.NotificationStatus.ToString()

                  })
                  .Where(a => a.NotificationStatus == "Unread")
                  .ToList();
            var read = (await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("Notifications")
                  .Child(user.LocalId)
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      Notification = item.Object.Notification.ToString(),
                      Time = item.Object.Time.ToString(),
                      NotificationStatus = item.Object.NotificationStatus.ToString()

                  })
                  .Where(a => a.NotificationStatus == "Read")
                  .ToList();

            foreach (var element in unread)
            {
                read.Insert(0, element);
            }



            return read
             .OrderByDescending(d => Convert.ToDateTime(d.Time).Year)
            .ThenByDescending(d => Convert.ToDateTime(d.Time).Date).ToList();

        }
        /*public static async Task<List<Person>> GetNotificationsNures(string city, string school, string email)
        {
            string CitySchool = city.ToLower() + school.ToLower();

            var unread = (await firebase
                  
                  .Child(CitySchool.Replace("" + " ", ""))
                  .Child("Notifications")
                  .Child(email.ToLower().Replace(".", ","))
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      Notification = "(New) " + item.Object.Notification.ToString(),
                      Time = item.Object.Time.ToString(),
                      NotificationStatus = item.Object.NotificationStatus.ToString()

                  })
                  .Where(a => a.NotificationStatus == "Unread")
                  .ToList();
           
            var read = (await firebase
                  
                  .Child(CitySchool.Replace("" + " ", ""))
                  .Child("Notifications")
                  .Child(email.ToLower().Replace(".", ","))
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      Notification = item.Object.Notification.ToString(),
                      Time = item.Object.Time.ToString(),
                      NotificationStatus = item.Object.NotificationStatus.ToString(),
                  })
                  .Where(a => a.NotificationStatus == "Read")
                  .ToList();
            


            if (unread.Count() != 0)
            {
                foreach (var element in unread)
                {
                    read.Insert(0, element);
                }

            }

            return read
             .OrderByDescending(d => Convert.ToDateTime(d.Time).Year)
            .ThenByDescending(d => Convert.ToDateTime(d.Time).Date).ToList();




        }*/
        public static async Task<List<Person>> GetNotificationsNurses(string city, string school, string email)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });

            string CitySchool = city.ToLower() + school.ToLower();

            var unread = (await firebaseClient
                  .Child(CitySchool.Replace("" + " ", ""))
                  .Child("Notifications")
                  .Child(user.LocalId)
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      Notification = "(New) " + item.Object.Notification.ToString(),
                      Time = item.Object.Time.ToString(),
                      NotificationStatus = item.Object.NotificationStatus.ToString()

                  })
                  .Where(a => a.NotificationStatus == "Unread")
                  .ToList();

            var read = (await firebaseClient
                  .Child(CitySchool.Replace("" + " ", ""))
                  .Child("Notifications")
                  .Child(user.LocalId)
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      Notification = item.Object.Notification.ToString(),
                      Time = item.Object.Time.ToString(),
                      NotificationStatus = item.Object.NotificationStatus.ToString(),
                  })
                  .Where(a => a.NotificationStatus == "Read")
                  .ToList();



            if (unread.Count() != 0)
            {
                foreach (var element in unread)
                {
                    read.Insert(0, element);
                }

            }

            return read
             .OrderByDescending(d => Convert.ToDateTime(d.Time).Year)
            .ThenByDescending(d => Convert.ToDateTime(d.Time).Date).ToList();




        }
        public static async Task<List<Person>> GetPastEntered()
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            string city = await GetPersonCity();
            string school = await GetPersonSchool();
            string CitySchool = city.ToLower() + school.ToLower();

            var unread = (await firebaseClient
                  .Child("PersonEnteredInfo")
                  .Child(user.LocalId)
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      Notification = item.Object.Notification,
                      Time = item.Object.Time

                  })
                  .ToList();
            return unread;




        }
        /*public static async Task<List<string>> GetAllNotificationMessages(string city, string school, string email)
        {
            string CitySchool = city.ToLower() + school.ToLower();

            var notifs = (await firebase
                  
                  .Child(CitySchool.Replace("" + " ", ""))
                  .Child("Notifications")
                  .Child(email.ToLower().Replace(".", ","))
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      Notification = item.Object.Notification.ToString()

                  })
                  .Where(a => a.Notification.ToLower().Contains("push notifications"))
                  .ToList();
            var list = new List<string>();
           foreach(var notif in notifs)
            {
                list.Add(notif.Notification.ToString());
            }
            return list;
        }*/
        public static async Task<List<string>> GetAllNotificationMessages(string city, string school, string email)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            string CitySchool = city.ToLower() + school.ToLower();

            var notifs = (await firebaseClient

                  .Child(CitySchool.Replace("" + " ", ""))
                  .Child("Notifications")
                  .Child(user.LocalId)
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      Notification = item.Object.Notification.ToString()

                  })
                  .Where(a => a.Notification.ToLower().Contains("push notifications"))
                  .ToList();
            var list = new List<string>();
            foreach (var notif in notifs)
            {
                list.Add(notif.Notification.ToString());
            }
            return list;
        }

        /*public static Task<List<Person>> DateTimesforSorting(List<Person> read)
        {
            var sortedReadings = read.OrderBy(x => Convert.ToDateTime(x.Time).Date)
                .ThenBy(x => Convert.ToDateTime(x.Time).Date)
                .ThenBy(x => Convert.ToDateTime(x.Time).Year)
                .ToList()
                ;

            return sortedReadings;
        }*/





        /*public static async Task GetClassAllergiesInfo(string city, string school, string classcode)
        {
            string CitySchool = city.ToLower() + school.ToLower();

                var allergies = await firebase.Child(CitySchool.Replace(" ","")).Child(classcode).Child("FoodAllergies").OrderByKey().OnceAsync<Allergy>(); 
                await App.Current.MainPage.DisplayAlert("Alert", allergies.Count().ToString(), "Ok");
                foreach (var allergy in allergies)
                {
                    Console.WriteLine($"{allergy.Key}");
                }
             

        }*/

        public static async Task<Person> GetPerson()
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            /*var allPersons = await GetAllPersons();

            await firebase
              .Child("Persons")
              .OnceAsync<Person>();
            var personthatfitscriteria = allPersons.Where(user => user.NewUserEmail == Username).ToList();
            return personthatfitscriteria;*/
            Person person = await firebaseClient
            .Child("Persons")
            .Child(user.LocalId)
            //.OrderBy("NewUserEmail".Replace(".", ","))
            //.EqualTo(Username)
            .OnceSingleAsync<Person>();
            return person;


        }
        public static async Task<string> GetPersonCity()
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });

            Person person = await firebaseClient
            .Child("Persons")
            .Child(user.LocalId)
            .OnceSingleAsync<Person>();
            if (person != null)
            {
                return person.City;

            }
            else
            {
                return "false";
            }


        }
        public static async Task<string> GetPersonName(string cityschool)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });

            Person person = await firebaseClient
            .Child(cityschool)
            .Child(user.LocalId)
            .OnceSingleAsync<Person>();
            return person.Name;

        }
        public static async Task<string> GetEmail()
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            return user.Email;


        }
        //
        public static async Task<List<string>> GetPersonStudentName()
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            var list = new List<string>();
            string city = await GetPersonCity();
            string school = await GetPersonSchool();
            string CitySchool = city.ToLower() + school.ToLower();

            List<Person> person = (await firebaseClient
                .Child(CitySchool.Replace(" ", ""))
            .Child("ClassesSignUps")
            .Child(user.LocalId)
            .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      Name = item.Object.Name.ToString()

                  })
                  .ToList();
            foreach (var code in person)
            {
                if (list.Contains(code.Name))
                {

                }
                else
                {
                    list.Add(code.Name);
                }
            }
            return list;

        }
        public static async Task<List<string>> GetPersonsClassCodes()
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;


            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            Person person1 = await firebaseClient
            .Child("Persons")
            .Child(user.LocalId)
            .OnceSingleAsync<Person>();

            var list = new List<string>();
            List<Person> person = (await firebaseClient
            .Child(((person1.City + person1.School).ToLower()).Replace(" ", ""))
            .Child("ClassesSignUps")
            .Child(user.LocalId)
            .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      ClassCode = item.Object.ClassCode.ToString()

                  })
                  .ToList();
            foreach (var code in person)
            {
                list.Add(code.ClassCode);
            }
            return list;

        }
        public static async Task<List<string>> GetPersonsClassCodesFromName(string name)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            var list = new List<string>();
            Person person1 = await firebaseClient
            .Child("Persons")
            .Child(user.LocalId)
            .OnceSingleAsync<Person>();

            List<Person> person = (await firebaseClient
            .Child(((person1.City + person1.School).ToLower()).Replace(" ", ""))
            .Child("ClassesSignUps")
            .Child(user.LocalId)
            .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      ClassCode = item.Object.ClassCode.ToString(),
                      Name = item.Object.Name

                  })
                  .Where(a => a.Name == name)
                  .ToList();
            foreach (var code in person)
            {
                list.Add(code.ClassCode);
            }
            return list;

        }
        public static async Task<string> GetPersonSchool()
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            Person person = await firebaseClient
            .Child("Persons")
            .Child(user.LocalId)
            .OnceSingleAsync<Person>();
            return person.School;


        }

        public static async Task<string> GetPersonClassCode(string Username)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });

            Person person = await firebaseClient
            .Child("Persons")
            .Child(Username.Replace(".", ","))
            .OnceSingleAsync<Person>();
            string classcode = person.ClassCode;
            string city = person.City;
            string school = person.School;
            string cityschoolclasscode = (city.ToLower() + school.ToLower() + classcode.ToLower()).Replace(" ", "");
            return cityschoolclasscode;

        }
        public static async Task<string> GetPersonEmail()
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            return Email;

        }
        public static async Task<string> GetClassCodeNotFormatted(string Username)
        {

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            Person person = await firebaseClient
            .Child("Persons")
            .Child(Username.Replace(".", ","))
            .OnceSingleAsync<Person>();
            string classcode = person.ClassCode;
            return classcode;

        }
        //Add person, create account
        public static async Task AddPerson(string role, string name, string school, string city, string UserEmail, string NewUserPassword)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(UserEmail, NewUserPassword);
            string gettoken = auth.FirebaseToken;
            await App.Current.MainPage.DisplayAlert("Sign Up Successful", " ", "Ok");
            var content = await auth.GetFreshAuthAsync();
            var serializedcontent = JsonConvert.SerializeObject(content);
            Preferences.Set("MyFirebaseRefreshToken", serializedcontent);

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken)
                            });
            await firebaseClient
              .Child("Persons")
              .Child(auth.User.LocalId)
              .PutAsync(new Person() { School = school, City = city, Role = role });
            await firebaseClient
              .Child(((city + school).ToLower()).Replace(" ", ""))
              .Child(auth.User.LocalId)
              .PutAsync(new Person() { NewUserEmail = UserEmail, Name = name });
            Person numbers = await firebaseClient
                  .Child((city+school).ToLower().Replace(" ", ""))
                  .Child("Viral Flu Numbers")
                  .OnceSingleAsync<Person>();
            Person numbers2 = await firebaseClient
                  .Child((city + school).ToLower().Replace(" ", ""))
                  .Child("COVID Numbers")
                  .OnceSingleAsync<Person>();
            if (numbers == null)
            {
                await firebaseClient
                 .Child((city+school).ToLower().Replace(" ", ""))
                 .Child("Viral Flu Numbers")
                 .PutAsync(new Person() { NumberOfCases = 0.ToString(), Time = "None Entered" });

            }
            if (numbers2 == null)
            {
                await firebaseClient
                 .Child((city + school).ToLower().Replace(" ", ""))
                 .Child("COVID Numbers")
                 .PutAsync(new Person() { NumberOfCases = 0.ToString(), Time = "None Entered" });

            }

        }




        public static async Task AddPersonWithoutAuth(string role, string name, string school, string city, string UserEmail, string NewUserPassword, FirebaseAuthLink auth)
        {

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken)
                            });
            await firebaseClient
              .Child("Persons")
              .Child(auth.User.LocalId)
              .PutAsync(new Person() { School = school, City = city, Role = role });
            await firebaseClient
              .Child(((city + school).ToLower()).Replace(" ", ""))
              .Child(auth.User.LocalId)
              .PutAsync(new Person() { NewUserEmail = UserEmail, Name = name });


        }
        /*public static async Task GetPeopleInSchoolAndAddNotifsCOVID(string school, string city, string VirusCase)
        {
            string CitySchool = city.ToLower() + school.ToLower();

            var listofpeople =  (await firebase
                  .Child("Persons")
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      NewUserEmail = item.Object.NewUserEmail,
                      School = item.Object.School,
                      City = item.Object.City

                  }).Where(a => ((a.City.ToLower() + a.School.ToLower()).Replace(" ", "")) == CitySchool.ToLower().Replace(" ", ""))

                  .ToList();
            foreach(var person in listofpeople)
            {
                await firebase
              
              .Child(CitySchool.ToLower().Replace(" ", ""))
              .Child("Notifications")
              .Child(person.NewUserEmail.ToLower().Replace(".",","))
              .PostAsync(new Person() { Notification = VirusCase + " case in your school", NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });

            }

        }*/

        //Add notifs for all people in school
        public static async Task GetPeopleInSchoolAndAddNotifsCOVID(string school, string city, string VirusCase, string studentname)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });

            string CitySchool = city.ToLower() + school.ToLower();

            var listofpeople = (await firebaseClient
                  .Child("Persons")
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      School = item.Object.School,
                      City = item.Object.City,
                      UID = item.Key

                  }).Where(a => ((a.City.ToLower() + a.School.ToLower()).Replace(" ", "")) == CitySchool.ToLower().Replace(" ", ""))

                  .ToList();
            foreach (var person in listofpeople)
            {
                await firebaseClient
              .Child(CitySchool.ToLower().Replace(" ", ""))
              .Child("Notifications")
              .Child(person.UID)
              .PostAsync(new Person() { Notification = VirusCase + " case in your school", NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });

            }




            var nurse = (await firebaseClient
                  .Child("Persons")
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      School = item.Object.School,
                      City = item.Object.City,
                      UID = item.Key,
                      Role = item.Object.Role

                  }).Where(a => ((a.City.ToLower() + a.School.ToLower()).Replace(" ", "")) == CitySchool.ToLower().Replace(" ", "")).Where(a => a.Role == "Nurse")

                  .ToList();
            await firebaseClient
            .Child("PersonEnteredInfo")
            .Child(user.LocalId)

            .PostAsync(new Person() { Time = DateTime.Now.ToString("MM/dd/yyyy"), Notification = "Entered COVID19 case" });
            foreach (var person in nurse)
            {
                Person info = await FirebaseHelper.GetPerson();
                if (info.Role == "Parent")
                {
                    await firebaseClient
              .Child(CitySchool.ToLower().Replace(" ", ""))
              .Child("NurseInfo")
              .Child(person.UID)
              .Child("VirusCases")
              .PostAsync(new Person() { NewUserEmail = user.Email, Name = studentname, Time = DateTime.Now.ToString("MM/dd/yyyy"), ParentName = await FirebaseHelper.GetPersonName(CitySchool.ToLower().Replace(" ", "")) });
                }
                else
                {
                    await firebaseClient
             .Child(CitySchool.ToLower().Replace(" ", ""))
             .Child("NurseInfo")
             .Child(person.UID)
             .Child("VirusCases")
             .PostAsync(new Person() { NewUserEmail = user.Email, Name = await FirebaseHelper.GetPersonName(CitySchool.ToLower().Replace(" ", "")), Time = DateTime.Now.ToString("MM/dd/yyyy"), ParentName = "Teacher" });
                }



            }


            Person numbers = await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("COVID Numbers")
                  .OnceSingleAsync<Person>();
            if (numbers != null)
            {
                await firebaseClient
             .Child(CitySchool.ToLower().Replace(" ", ""))
             .Child("COVID Numbers")
             .PutAsync(new Person() { NumberOfCases = (System.Convert.ToInt32(numbers.NumberOfCases) + 1).ToString(), Time = DateTime.Now.ToString("MM/dd/yyyy") });
            }
            else
            {
                await firebaseClient
             .Child(CitySchool.ToLower().Replace(" ", ""))
             .Child("COVID Numbers")
             .PutAsync(new Person() { NumberOfCases = "1", Time = DateTime.Now.ToString("MM/dd/yyyy") });
            }

        }

        public static async Task<string> GetNumberOfPeopleInSchoolRegistered(string school, string city, string role)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            string CitySchool = city + school;
            var people = (await firebaseClient
                 .Child("Persons")
                 .OnceAsync<Person>())
                 .Select(item => new Person
                 {
                     School = item.Object.School,
                     City = item.Object.City,

                     Role = item.Object.Role

                 }).Where(a => ((a.City.ToLower() + a.School.ToLower()).Replace(" ", "")) == CitySchool.ToLower().Replace(" ", "")).Where(a => a.Role == role);
            return people.Count().ToString();
        }

        //Find school nurse
        public static async Task GetNurseAddNotifs(string school, string city, string allergy, string name)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });

            string CitySchool = city.ToLower() + school.ToLower();

            var listofpeople = (await firebaseClient
                  .Child("Persons")
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      UID = item.Key,
                      School = item.Object.School,
                      City = item.Object.City,
                      Role = item.Object.Role

                  }).Where(a => ((a.City.ToLower() + a.School.ToLower()).Replace(" ", "")) == CitySchool.ToLower().Replace(" ", "")).Where(a => a.Role == "Nurse")

                  .ToList();
            foreach (var person in listofpeople)
            {
                await firebaseClient
              .Child(CitySchool.ToLower().Replace(" ", ""))
              .Child("Notifications")
              .Child(person.UID)
              .PostAsync(new Person() { Notification = "Medical Allergy: " + allergy + " (" + name + ")", NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });

            }

        }

        public static async Task GetPeopleInClassAndAddNotifsforHeadLiceCases(string school, string city, string viruscase, string classcode)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });

            string CitySchool = city.ToLower() + school.ToLower();


            var nurse = (await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("ClassInfo")
                  .Child(classcode.ToLower())
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      UID = item.Key,

                  })

                  .ToList();
            foreach (var person in nurse)
            {
                await firebaseClient
              .Child(CitySchool.Replace(" ", ""))
              .Child("Notifications")
              .Child(person.UID)
              .PostAsync(new Person() { Notification = "Head lice case reported in " + classcode, NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });

            }


        }

        public static async Task SendMessageToClass(string school, string city, string message, string classcode, string name, string receiver)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            string CitySchool = city.ToLower() + school.ToLower();

            await firebaseClient
          .Child(CitySchool.ToLower().Replace(" ", ""))
          .Child("Messages")
          .PostAsync(new CustomMessageToSend() { MessageText = message, MessageStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy H:mm"), From = name, To = receiver });

            var listofpeople = (await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("ClassInfo")
                  .Child(classcode.ToLower())
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      UID = item.Key,


                  })
                  .ToList();

            foreach (var person in listofpeople)
            {
                await firebaseClient

              .Child(CitySchool.ToLower().Replace(" ", ""))
              .Child("Notifications")
              .Child(person.UID)
              .PostAsync(new Person() { Notification = "From: " + name + ", " + message, NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });

            }

        }
        public static async Task SendMessageToSchool(string school, string city, string message, string name)
        {
            string CitySchool = city.ToLower() + school.ToLower();

            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });

            var listofpeople = (await firebaseClient
                              .Child("Persons")
                              .OnceAsync<Person>())
                              .Select(item => new Person
                              {
                                  School = item.Object.School,
                                  City = item.Object.City

                              }).Where(a => ((a.City.ToLower() + a.School.ToLower()).Replace(" ", "")) == CitySchool.ToLower().Replace(" ", ""))

                              .ToList();
            foreach (var person in listofpeople)
            {
                await firebaseClient

              .Child(CitySchool.ToLower().Replace(" ", ""))
              .Child("Notifications")
              .Child(user.LocalId)
              .PostAsync(new Person() { Notification = message + " - From: " + name, NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });

            }
            await firebaseClient

              .Child(CitySchool.Replace(" ", ""))
              .Child("Messages")
              .PostAsync(new CustomMessageToSend() { MessageText = message, MessageStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy H:mm"), From = name, To = CitySchool.Replace(" ", "") });


        }
        public static async Task SendMessageToTeachers(string school, string city, string message, string name)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            string CitySchool = city.ToLower() + school.ToLower();

            var nurse = (await firebaseClient
                .Child("Persons")
                .OnceAsync<Person>())
                .Select(item => new Person
                {
                    Role = item.Object.Role,
                    School = item.Object.School,
                    City = item.Object.City,
                }).Where(a => ((a.City.ToLower() + a.School.ToLower()).Replace(" ", "")) == CitySchool.ToLower().Replace(" ", "")).Where(a => a.Role == "Teacher");

            foreach (var person in nurse)
            {
                await firebaseClient

              .Child(CitySchool.ToLower().Replace(" ", ""))
              .Child("Notifications")
              .Child(user.LocalId)
              .PostAsync(new Person() { Notification = "From: " + name + message, NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });

            }

            await firebaseClient

          .Child(CitySchool.Replace(" ", ""))
          .Child("Messages")
          .PostAsync(new CustomMessageToSend() { MessageText = message, MessageStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy H:mm"), From = name, To = "Teachers" });


        }
        public static async Task AddNurseVerificationCode(string school, string city, string code)
        {
            string CitySchool = city.ToLower() + school.ToLower();

            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            await firebaseClient
          .Child(CitySchool.Replace(" ", ""))
          .Child("VerificationCode")
          .PutAsync(new Person() { ClassCode = code });


        }
        public static async Task<string> GetNurseVerificationCode(string school, string city)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            string CitySchool = city.ToLower() + school.ToLower();

            Person schoolinfo = await firebaseClient
            .Child(CitySchool.Replace(" ", ""))
            .Child("VerificationCode")
            .OnceSingleAsync<Person>();

            if (schoolinfo.ClassCode != null)
            {
                return schoolinfo.ClassCode.ToLower();
            }
            else
            {
                return "Code not generated, please contact your administrator to generate it";
            }



        }

        public static async Task AddOtherNotification(string school, string city, string notification, string email)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });

            string CitySchool = city.ToLower() + school.ToLower();


            await firebaseClient

          .Child(CitySchool.ToLower().Replace(" ", ""))
          .Child("Notifications")
          .Child(user.LocalId)
          .PostAsync(new Person() { Notification = notification, NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });


        }
        public static async Task GetPeopleInClassAndAddNotifsforAllergy(string school, string city, string Allergy, string classcode, string allergydetails)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });


            string CitySchool = city.ToLower() + school.ToLower();

            var peopleinclass = (await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("ClassInfo")
                  .Child(classcode.ToLower())
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      UID = item.Key

                  })
                  .ToList();

            foreach (var Uid in peopleinclass)
            {
                await firebaseClient
              .Child(CitySchool.ToLower().Replace(" ", ""))
              .Child("Notifications")
              .Child(Uid.UID)
              .PostAsync(new Person() { Notification = "Allergy: " + Allergy + " (" + allergydetails + ")", NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });
            }
        }
        public static async Task GetPeopleInClassAndAddNotifsforAllergyTeacher(string school, string city, string Allergy, string classcode, string allergydetails)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });

            string CitySchool = city.ToLower() + school.ToLower();

            var teacher = await firebaseClient
            .Child(CitySchool.Replace(" ", ""))
            .Child("TeacherUID")
            .Child(classcode)
            .OnceSingleAsync<Person>();


            await firebaseClient
          .Child(CitySchool.ToLower().Replace(" ", ""))
          .Child("Notifications")
          .Child(teacher.UID)
          .PostAsync(new Person() { Notification = "Allergy: " + Allergy + " (" + allergydetails + ")", NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });



        }
        public static async Task AddVirusCaseInfo(string role, string name, string school, string city, string NewUserEmail, string viruscase, string classcode)
        {
            string CitySchool = city.ToLower() + school.ToLower();

            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            var listofpeople = (await firebaseClient
                  .Child("Persons")
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      UID = item.Key,
                      School = item.Object.School,
                      City = item.Object.City,
                      Role = item.Object.Role

                  }).Where(a => ((a.City.ToLower() + a.School.ToLower()).Replace(" ", "")) == CitySchool.ToLower().Replace(" ", "")).Where(a => a.Role == "Nurse")

                  .ToList();

            await firebaseClient
            .Child("PersonEnteredInfo")
            .Child(user.LocalId)
            .PostAsync(new Person() { Time = DateTime.Now.ToString("MM/dd/yyyy"), Notification = "Virus Case Entered: " + viruscase });

            foreach (var person in listofpeople)
            {
                await firebaseClient
             .Child(CitySchool.Replace(" ", ""))
             .Child("NurseInfo")
             .Child(person.UID)
             .Child("HealthInfoVirusCases")
             .PostAsync(new Person() { NewUserEmail = NewUserEmail, Role = role, Name = name, ClassCode = classcode.ToLower(), HealthInfoVirusCase = viruscase, Time = DateTime.Now.ToString("MM/dd/yyyy") });
            }

            if (viruscase == "Viral Flu")
            {
                Person numbers = await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("Viral Flu Numbers")
                  .OnceSingleAsync<Person>();
                await firebaseClient
                 .Child(CitySchool.ToLower().Replace(" ", ""))
                 .Child("Viral Flu Numbers")
                 .PutAsync(new Person() { NumberOfCases = (System.Convert.ToInt32(numbers.NumberOfCases) + 1).ToString(), Time = DateTime.Now.ToString("MM/dd/yyyy") });
            }


        }

        public static async Task MarkAsRead(string city, string school, string email)
        {
            string CitySchool = city.ToLower() + school.ToLower();

            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });

            var allunreadnotifs = (await firebaseClient
                  .Child(CitySchool.Replace("" + " ", ""))
                  .Child("Notifications")
                  .Child(user.LocalId)
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      Notification = item.Object.Notification.ToString(),
                      FirebaseKey = item.Key,
                      Time = item.Object.Time.ToString(),
                      NotificationStatus = item.Object.NotificationStatus.ToString()

                  })
                  .Where(a => a.NotificationStatus == "Unread")
                  .ToList();

            foreach (var notif in allunreadnotifs)
            {
                await firebaseClient

                .Child(CitySchool.Replace("" + " ", ""))
                .Child("Notifications")
                .Child(user.LocalId)
                .Child(notif.FirebaseKey)
                .PutAsync(new Person() { Time = notif.Time, NotificationStatus = "Read", Notification = notif.Notification });
            }

        }


        public static async Task<Person> GetStudentName(string city, string classcode, string school, string name)
        {
            string CitySchool = city.ToLower() + school.ToLower();
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            Person person = await firebaseClient
            .Child(CitySchool)
            .Child(classcode)
            .Child(name)
            .OnceSingleAsync<Person>();
            return person;
        }

        public static async Task AddAllergy(string name, string role, string school, string city, string classcode, string foodallergies, string email, string allergydetails, string permission, string parentname)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            string CitySchool = city.ToLower() + school.ToLower();

            var teacher = await firebaseClient
            .Child(CitySchool.Replace(" ", ""))
            .Child("TeacherUID")
            .Child(classcode.ToLower())
            .OnceSingleAsync<Person>();

            var schoolnurse = (await firebaseClient
                  .Child("Persons")
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      City = item.Object.City.ToString(),
                      School = item.Object.School,
                      Role = item.Object.Role.ToString(),
                      UID = item.Key

                  })
                  .Where(a => a.Role == "Nurse").Where(a => (a.City + a.School).ToLower().Replace(" ", "") == CitySchool.Replace(" ", ""))
                  .ToList();

            if (permission == "Yes, I want other parents to be notified/shown this information")
            {

                var peopleinclass = (await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("ClassInfo")
                  .Child(classcode.ToLower())
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      UID = item.Key

                  })
                  .ToList();

                foreach (var Uid in peopleinclass)
                {
                    await firebaseClient
                  .Child(CitySchool.ToLower().Replace(" ", ""))
                  .Child("Notifications")
                  .Child(Uid.UID)
                  .PostAsync(new Person() { Notification = "Allergy: " + foodallergies + " (" + allergydetails + ")", NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });
                }

                await firebaseClient
              .Child(CitySchool.Replace(" ", ""))
              .Child("ParentInfo")
              .Child(classcode.ToLower())
              .Child("FoodAllergies")
              .PostAsync(new Person() { FoodAllergies = foodallergies, AllergyDetails = allergydetails });

                await firebaseClient
              .Child(CitySchool.Replace(" ", ""))
              .Child("TeacherInfo")
              .Child(classcode.ToLower())
              .Child(teacher.UID)
              .Child("FoodAllergies")
              .PostAsync(new Person() { FoodAllergies = foodallergies, AllergyDetails = allergydetails, Name = name, NewUserEmail = email, ClassCode = classcode });

                foreach (var person in schoolnurse)
                {
                    if (role == "Teacher")
                    {
                        await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("NurseInfo")
                  .Child(person.UID)
                  .Child("FoodAllergies")
                  .PostAsync(new Person() { FoodAllergies = foodallergies, AllergyDetails = allergydetails, ClassCode = classcode, NewUserEmail = email, Name = name, ParentName = "Teacher" });
                    }
                    else
                    {
                        await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("NurseInfo")
                  .Child(person.UID)
                  .Child("FoodAllergies")
                  .PostAsync(new Person() { FoodAllergies = foodallergies, AllergyDetails = allergydetails, ClassCode = classcode, NewUserEmail = email, Name = name, ParentName = parentname });
                    }
                }


            }
            else
            {
                await firebaseClient
              .Child(CitySchool.Replace(" ", ""))
              .Child("TeacherInfo")
              .Child(classcode.ToLower())
              .Child(teacher.UID)
              .Child("FoodAllergies")
              .PostAsync(new Person() { FoodAllergies = foodallergies, AllergyDetails = allergydetails, Name = name, NewUserEmail = email, ClassCode = classcode });
                foreach (var person in schoolnurse)
                {
                    if (role == "Teacher")
                    {
                        await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("NurseInfo")
                  .Child(person.UID)
                  .Child("FoodAllergies")
                  .PostAsync(new Person() { FoodAllergies = foodallergies, AllergyDetails = allergydetails, ClassCode = classcode, NewUserEmail = email, Name = name, ParentName = "Teacher" });
                    }
                    else
                    {
                        await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("NurseInfo")
                  .Child(person.UID)
                  .Child("FoodAllergies")
                  .PostAsync(new Person() { FoodAllergies = foodallergies, AllergyDetails = allergydetails, ClassCode = classcode, NewUserEmail = email, Name = name, ParentName = parentname });
                    }
                }



                await firebaseClient
              .Child(CitySchool.ToLower().Replace(" ", ""))
              .Child("Notifications")
              .Child(teacher.UID)
              .PostAsync(new Person() { Notification = "Allergy: " + foodallergies + " (" + allergydetails + ")", NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });
            }

            await firebaseClient
             .Child("PersonEnteredInfo")
             .Child(user.LocalId)

             .PostAsync(new Person() { Time = DateTime.Now.ToString("MM/dd/yyyy"), Notification = "Allergy Entered: " + foodallergies });


        }

        public static async Task AddMedicalAllergyStudent(string name, string role, string StudentName, string school, string city, string classcode, string medicalallergy, string email)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });

            var schoolnurse = (await firebaseClient
                  .Child("Persons")
                  .Child(user.LocalId)
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      City = item.Object.City.ToString(),
                      School = item.Object.School,
                      Role = item.Object.Role.ToString(),
                      UID = item.Key

                  })
                  .Where(a => a.Role == "Nurse").Where(a => (a.City + a.School).ToLower().Replace(" ", "") == (city.ToLower() + school.ToLower()).Replace(" ", ""))
                  .ToList();
            string CitySchool = city.ToLower() + school.ToLower();

            foreach (var person in schoolnurse)
            {
                await firebaseClient
                .Child(CitySchool.Replace(" ", ""))
                .Child("NurseInfo")
                .Child(person.UID)
                .Child("MedicalAllergies")
                .PostAsync(new Person() { NewUserEmail = email, MedicalAllergies = medicalallergy.ToLower(), ParentName = name, Name = StudentName, ClassCode = classcode.ToLower(), Role = role });

                await firebaseClient
                .Child(CitySchool.Replace(" ", ""))
                .Child("Notifications")
                .Child(person.UID)
                .PostAsync(new Person() { Notification = "Medical Allergy Entered in " + classcode, Time = DateTime.Now.ToString("MM/dd/yyyy") });
            }




        }
        public static async Task AddMedicalAllergyTeacher(string name, string role, string school, string city, string classcode, string medicalallergy, string email)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            string CitySchool = city.ToLower() + school.ToLower();
            var schoolnurse = (await firebaseClient
                  .Child("Persons")
                  .Child(user.LocalId)
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      City = item.Object.City.ToString(),
                      School = item.Object.School,
                      Role = item.Object.Role.ToString(),
                      UID = item.Key

                  })
                  .Where(a => a.Role == "Nurse").Where(a => (a.City + a.School).ToLower().Replace(" ", "") == (city.ToLower() + school.ToLower()).Replace(" ", ""))
                  .ToList();
            foreach (var person in schoolnurse)
            {
                await firebaseClient
                .Child((city + school).ToLower().Replace(" ", ""))
                .Child("NurseInfo")
                .Child(person.UID)
                .Child("MedicalAllergies")
                .PostAsync(new Person() { NewUserEmail = email, MedicalAllergies = medicalallergy.ToLower(), ParentName = "Teacher", Name = name, ClassCode = classcode.ToLower(), Role = role });

                await firebaseClient
                .Child((city + school).ToLower().Replace(" ", ""))
                .Child("Notifications")
                .Child(person.UID)
                .PostAsync(new Person() { Notification = "Medical Allergy Entered in " + classcode, Time = DateTime.Now.ToString("MM/dd/yyyy") });
            }



        }


        public static async Task AddClass(string classcode, string name, string school, string city, string email, string role)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            string CitySchool = city.ToLower() + school.ToLower();
            await firebaseClient
              .Child(CitySchool.Replace(" ", ""))
              .Child("ClassesSignUps")
              .Child(user.LocalId)
              .PostAsync(new Person() { ClassCode = classcode, Name = name });

           

            await firebaseClient
              .Child(CitySchool.Replace(" ", ""))
              .Child("ClassInfo")
              .Child(classcode.ToLower())
              .Child(user.LocalId)
              .PutAsync(new Person() { Name = name });

            await firebaseClient
                .Child(CitySchool.Replace(" ", ""))
                .Child("TeacherUID")
              .Child(classcode.ToLower())
              .PutAsync(new Person() { UID = user.LocalId });

            var schoolnurse = (await firebaseClient
                .Child("Persons")
                .OnceAsync<Person>())
                .Select(item => new Person
                {
                    City = item.Object.City.ToString(),
                    School = item.Object.School,
                    Role = item.Object.Role.ToString(),
                    UID = item.Key

                })
                .Where(a => a.Role == "Nurse").Where(a => (a.City + a.School).ToLower().Replace(" ", "") == CitySchool.Replace(" ", ""))
                .ToList();
            foreach (var person in schoolnurse)
            {
                await firebaseClient
            .Child(CitySchool.Replace(" ", ""))
            .Child("Classes")
            .Child(user.LocalId)
            .PostAsync(new Person() { ClassCode = classcode });

                if (role == "Teacher")
                {
                    await firebaseClient
              .Child(CitySchool.Replace(" ", ""))
             .Child("ClassInfo")
             .Child(classcode.ToLower())
             .Child(person.UID)
              .PutAsync(new Person() { Name = name });
                }
                else
                {
                    await firebaseClient
              .Child(CitySchool.Replace(" ", ""))
             .Child("ClassInfo")
             .Child(classcode.ToLower())
                .Child(person.UID)
              .PutAsync(new Person() { Name = name });
                }
            }

        }
        public static async Task AddToClass(string classcode, string school, string name, string city, string email, string role)
        {
            string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            string UsersEmailToDisplay = savedfirebaseauth.User.Email;
            string Email = UsersEmailToDisplay;
            Firebase.Auth.User user = savedfirebaseauth.User;

            var firebaseClient = new FirebaseClient(
                            url,
                            new FirebaseOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                            });
            string CitySchool = city.ToLower() + school.ToLower();
            await firebaseClient
              .Child(CitySchool.Replace(" ", ""))
              .Child("ClassesSignUps")
              .Child(user.LocalId)
              .PostAsync(new Person() { ClassCode = classcode, Name = name });

            await firebaseClient
             .Child(CitySchool.Replace(" ", ""))
             .Child("ClassInfo")
             .Child(classcode.ToLower())
             .Child(user.LocalId)
             .PutAsync(new Person() { Name = name });

        }

    }
}

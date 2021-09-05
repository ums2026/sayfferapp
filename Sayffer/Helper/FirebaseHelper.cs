
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Sayffer.Model;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Sayffer.Helper
{
    public class FirebaseHelper
    {
        public static string url = "https://xamarinformsfirebase2-92714.firebaseio.com/";

        public static FirebaseClient firebase = new FirebaseClient(url);
        public static string Web_API_Key = "AIzaSyBjLg2kJqpKcECxDOdm2iQb6yz4utpVI5s";



        public static async Task<FirebaseClient> GetFirebaseClientUser()
            
        {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
                var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(await SecureStorage.GetAsync("MyFirebaseRefreshToken"));
                var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            await SecureStorage.SetAsync("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));

                var firebaseClient = new FirebaseClient(
                                url,
                                new FirebaseOptions
                                {
                                    AuthTokenAsyncFactory = () => Task.FromResult(RefreshedContent.FirebaseToken)
                                });
                return firebaseClient;
            
        }
        public static async Task<User> GetUser()
        {
            
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(await SecureStorage.GetAsync("MyFirebaseRefreshToken"));
            var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            await SecureStorage.SetAsync("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            Firebase.Auth.User user = savedfirebaseauth.User;
            return user;
            
            
        }

        public static async Task<List<Person>> GetClassAllergiesInfoParents(string city, string school, string classcode)
        {
       
                var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();
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
        public static async Task<List<Person>> GetClassAllergiesInfoNurse(string city, string school, string classcode)
        {

            try

            {
                Firebase.Auth.User user = await FirebaseHelper.GetUser();

                var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

                string CitySchool = city.ToLower() + school.ToLower();

                return (await firebaseClient
                      .Child(CitySchool.Replace(" ", ""))
                      .Child("NurseInfo")
                      .Child(user.LocalId)
                      .Child("FoodAllergies")
                      .OnceAsync<Person>())
                      .Select(item => new Person
                      {
                          FoodAllergies = item.Object.FoodAllergies.ToString(),
                          AllergyDetails = "Allergy severity: " + item.Object.AllergyDetails.ToString(),
                          ClassCode = item.Object.ClassCode,
                          NewUserEmail = item.Object.Name.ToString() + " (" + item.Object.NewUserEmail.ToString() + ")",
                          ParentName = "Parent Name ('Teacher' if user is a teacher):"+item.Object.ParentName.ToString()
                      })
                      .Where(a => a.ClassCode.ToLower() == classcode.ToLower())
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
        public static async Task<List<Person>> GetClassAllergiesInfo(string city, string school, string classcode)
        {

            try

            {
                Firebase.Auth.User user = await FirebaseHelper.GetUser();
                var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();


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
                      .Where(a => a.ClassCode.ToLower() == classcode.ToLower())
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

        public static async Task<List<string>> GetAllClassCodes(string cityschool)
        {
            var user = await FirebaseHelper.GetUser();
            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

            var classcodes = (await firebaseClient
            .Child(cityschool.ToLower().Replace(" ",""))
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

        public static async Task<List<Person>> GetMedicalAllergyInfoByClassTeachers(string city, string school, string classcode)
        {
            string CitySchool = city.ToLower() + school.ToLower();

            Firebase.Auth.User user = await FirebaseHelper.GetUser();
            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

            return (await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("NurseInfo")
                  .Child(user.LocalId)
                  .Child("MedicalAllergies")
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      MedicalAllergies = item.Object.MedicalAllergies.ToString(),
                      NewUserEmail = item.Object.NewUserEmail.ToString() + ", " + item.Object.Name.ToString(),
                      ClassCode = item.Object.ClassCode.ToString().ToLower(),
                      Role = item.Object.Role,
                      StudentName = " ",
                      ParentName=" "
                  }).Where(a => a.ClassCode == classcode.ToLower()).Where(a => a.Role.ToString().ToLower() == "teacher")

                  .ToList();

        }
        public static async Task<List<Person>> GetMedicalAllergyInfoByClassStudents(string city, string school, string classcode)
        {
            string CitySchool = city.ToLower() + school.ToLower();

            Firebase.Auth.User user = await FirebaseHelper.GetUser();
            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

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
                      ClassCode = item.Object.ClassCode.ToString().ToLower(),
                      Role = item.Object.Role.ToString()

                  }).Where(a => (a.ClassCode == classcode.ToLower())).Where(a=>a.Role.ToString() != "Teacher")

                  .ToList();

        }
        public static async Task<List<Person>> GetVirusCaseInfoByClass(string city, string school, string classcode)
        {
            string CitySchool = city.ToLower() + school.ToLower();
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

            return (await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("NurseInfo")
                  .Child(user.LocalId)
                  .Child("HealthInfoVirusCases")
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      HealthInfoVirusCase = item.Object.HealthInfoVirusCase.ToString() + " (" + item.Object.Time.ToString() + ")",
                      Name = item.Object.Name.ToString() + ", " + item.Object.Role.ToString(),
                      NewUserEmail = item.Object.NewUserEmail.ToString(),
                      ClassCode = item.Object.ClassCode,

                  }).Where(a => a.ClassCode.ToString().ToLower() == classcode.ToLower())
                  .ToList();


        }

        public static async Task<Person> GetCovidNumbersSchool(string city, string school)
        {
            string CitySchool = city.ToLower() + school.ToLower();
            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

            Person numbers = await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("COVID Numbers")
                  .OnceSingleAsync<Person>();
            return numbers;
            

        }
        public static async Task<Person> GetFluNumbersSchool(string city, string school)
        {
            string CitySchool = city.ToLower() + school.ToLower();
            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

            Person numbers = await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("Viral Flu Numbers")
                  .OnceSingleAsync<Person>();
            return numbers;

        }


      
        public static async Task<List<Person>> GetUnreadNotifications(string city, string school)
        {

            Firebase.Auth.User user = await FirebaseHelper.GetUser();
            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

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
       
        public static async Task<List<Person>> GetAllNotifications(string city, string school, string email)
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

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
       
    
        public static async Task<List<Person>> GetPastEntered()
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

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

            return unread
            .OrderByDescending(d => Convert.ToDateTime(d.Time).Year)
           .ThenByDescending(d => Convert.ToDateTime(d.Time).Date).ToList();


        }

     

        public static async Task<Person> GetPerson()
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();


            Person person = await firebaseClient
            .Child("Persons")
            .Child(user.LocalId)
            .OnceSingleAsync<Person>();
            return person;


        }
        public static async Task<string> GetPersonCity()
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();


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
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

            Person person = await firebaseClient
            .Child(cityschool)
            .Child(user.LocalId)
            .OnceSingleAsync<Person>();
            return person.Name;

        }
       
        
        public static async Task<List<string>> GetPersonStudentName()
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

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
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

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
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

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
                  .Where(a => a.Name.ToLower() == name.ToLower())
                  .ToList();
            foreach (var code in person)
            {
                list.Add(code.ClassCode);
            }
            return list;

        }
        public static async Task<string> GetPersonSchool()
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

            Person person = await firebaseClient
            .Child("Persons")
            .Child(user.LocalId)
            .OnceSingleAsync<Person>();
            return person.School;


        }

        
        public static async Task<string> GetPersonEmail()
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();
            return user.Email;

        }
        
        public static async Task AddPerson(string role, string name, string school, string city, string UserEmail, string NewUserPassword)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(UserEmail, NewUserPassword);
            string gettoken = auth.FirebaseToken;
            await App.Current.MainPage.DisplayAlert("Sign Up Successful", " ", "Ok");
            var content = await auth.GetFreshAuthAsync();
            var serializedcontent = JsonConvert.SerializeObject(content);
            await SecureStorage.SetAsync("MyFirebaseRefreshToken", serializedcontent);

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
            if(role == "Nurse")
            {
                await authProvider.SendEmailVerificationAsync(auth.FirebaseToken);
                await App.Current.MainPage.DisplayAlert("Check your email!", "Click the verification link and then come sign in!", "Ok!");
                SecureStorage.Remove("MyFirebaseRefreshToken");
            }

        }




        public static async Task GetPeopleInSchoolAndAddNotifsCOVID(string school, string city, string VirusCase, string studentname)
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();


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
            
            foreach (var person in nurse)
            {
                Person info = await FirebaseHelper.GetPerson();
                if (info.Role == "Parent")
                {
                    await firebaseClient
              .Child(CitySchool.ToLower().Replace(" ", ""))
              .Child("NurseInfo")
              .Child(person.UID)
              .Child("HealthInfoVirusCases")
              .PostAsync(new Person() { NewUserEmail = user.Email, Name = studentname, Time = DateTime.Now.ToString("MM/dd/yyyy"), ParentName = await FirebaseHelper.GetPersonName(CitySchool.ToLower().Replace(" ", "")) });
                }
                else
                {
                    await firebaseClient
             .Child(CitySchool.ToLower().Replace(" ", ""))
             .Child("NurseInfo")
             .Child(person.UID)
             .Child("HealthInfoVirusCases")
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
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();
            
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

        public static async Task GetNurseAddNotifs(string school, string city, string allergy, string name)
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

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

        public static async Task GetPeopleInClassAndAddNotifsforHeadLiceCases(string school, string city, string classcode)
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

            string CitySchool = city.ToLower() + school.ToLower();


            var people = (await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("ClassInfo")
                  .Child(classcode.ToLower())
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      UID = item.Key,

                  })

                  .ToList().Where(a => a.UID != user.LocalId);
            foreach (var person in people)
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
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

            string CitySchool = city.ToLower() + school.ToLower();
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


            var listofpeople = (await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("ClassInfo")
                  .Child(classcode.ToLower())
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      UID = item.Key,


                  })
                  .ToList().Where(a=>a.UID != schoolnurse[0].UID);
            
   
            foreach (var person in listofpeople)
            {
                await firebaseClient
              .Child(CitySchool.ToLower().Replace(" ", ""))
              .Child("Notifications")
              .Child(person.UID)
              .PostAsync(new Person() { Notification = "From " + name + ": " + message, NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });

            }

        }
        public static async Task SendMessageToSchool(string school, string city, string message, string name)
        {
            string CitySchool = city.ToLower() + school.ToLower();

            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();


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
              .PostAsync(new Person() { Notification = message + " - From: " + name, NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });

            }
            


        }
        public static async Task SendMessageToTeachers(string school, string city, string message, string name)
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

            string CitySchool = city.ToLower() + school.ToLower();

            var nurse = (await firebaseClient
                .Child("Persons")
                .OnceAsync<Person>())
                .Select(item => new Person
                {
                    Role = item.Object.Role,
                    School = item.Object.School,
                    City = item.Object.City,
                    UID = item.Key
                }).Where(a => ((a.City.ToLower() + a.School.ToLower()).Replace(" ", "")) == CitySchool.ToLower().Replace(" ", "")).Where(a => a.Role == "Teacher");

            foreach (var person in nurse)
            {
                await firebaseClient

              .Child(CitySchool.ToLower().Replace(" ", ""))
              .Child("Notifications")
              .Child(person.UID)
              .PostAsync(new Person() { Notification = "From " + name + ": " + message, NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });

            }

            


        }
        public static async Task SendMessageToParents(string school, string city, string message, string name)
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

            string CitySchool = city.ToLower() + school.ToLower();

            var parent = (await firebaseClient
                .Child("Persons")
                .OnceAsync<Person>())
                .Select(item => new Person
                {
                    Role = item.Object.Role,
                    School = item.Object.School,
                    City = item.Object.City,
                    UID = item.Key
                }).Where(a => ((a.City.ToLower() + a.School.ToLower()).Replace(" ", "")) == CitySchool.ToLower().Replace(" ", "")).Where(a => a.Role == "Parent");

            foreach (var person in parent)
            {
                await firebaseClient

              .Child(CitySchool.ToLower().Replace(" ", ""))
              .Child("Notifications")
              .Child(person.UID)
              .PostAsync(new Person() { Notification = "From " + name + ": " + message, NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });

            }




        }

        public static async Task SendMessageToAdmin(string school, string city, string message, string name)
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

            string CitySchool = city.ToLower() + school.ToLower();

            var admin = (await firebaseClient
                .Child("Persons")
                .OnceAsync<Person>())
                .Select(item => new Person
                {
                    Role = item.Object.Role,
                    School = item.Object.School,
                    City = item.Object.City,
                    UID = item.Key
                }).Where(a => ((a.City.ToLower() + a.School.ToLower()).Replace(" ", "")) == CitySchool.Replace(" ", "")).Where(a => a.Role == "Admin");

            foreach (var person in admin)
            {
                await firebaseClient

              .Child(CitySchool.ToLower().Replace(" ", ""))
              .Child("Notifications")
              .Child(person.UID)
              .PostAsync(new Person() { Notification = "From " + name + ": " + message, NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });

            }




        }

        public static async Task<string> GetRegisteredSchools(string school, string city, bool nurse, string email)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Web_API_Key));
            var auth = await authProvider.SignInAnonymouslyAsync();
            var content = await auth.GetFreshAuthAsync();
            var serializedcontent = JsonConvert.SerializeObject(content);
            await SecureStorage.SetAsync("MyFirebaseRefreshToken", serializedcontent);

            var firebaseClient = new FirebaseClient(
                                url,
                                new FirebaseOptions
                                {
                                    AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken)
                                });

            string CitySchool = city.ToLower() + school.ToLower();
            if(nurse == false)
            {
                try
                {
                    var schoolprofile = await firebaseClient
                     .Child("Registered Schools")
                     .Child(CitySchool.Replace(" ", "").ToLower())
                     .OnceSingleAsync<Person>();
                    if (schoolprofile == null)
                    {
                        App.Current.MainPage = new NavigationPage(new MainPage());
                        await App.Current.MainPage.DisplayAlert("Oops!", "Your school is not registered", "Ok");
                        return ("not registered");
                    }
                    else
                    {
                        return ("registered");

                    }

                }

                catch(Exception x)
                {

                    App.Current.MainPage = new NavigationPage(new MainPage());
                    await App.Current.MainPage.DisplayAlert("Oops!", "Your school is not registered", "Ok");
                    Crashes.TrackError(x);
                    return ("not registered");
                }
                   
            }
            else
            {
                var schoolprofile = await firebaseClient
                      .Child("Registered Schools")
                      .Child(((city + school).ToLower()).Replace(" ", ""))
                      .OnceSingleAsync<Person>();
                  
                if (schoolprofile.NewUserEmail.ToLower() == email.ToLower())
                {
                    return "Correct email adress";

                }
                else
                {
                    return "Incorrect email adress";
                }


            }


        }

        public static async Task AddViralFluNumbers(string school, string city)
        {
            string CitySchool = city.ToLower() + school.ToLower();

            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

            Person numbers = await firebaseClient
                  .Child(CitySchool.Replace(" ", ""))
                  .Child("Viral Flu Numbers")
                  .OnceSingleAsync<Person>();
            await firebaseClient
             .Child(CitySchool.ToLower().Replace(" ", ""))
             .Child("Viral Flu Numbers")
             .PutAsync(new Person() { NumberOfCases = (System.Convert.ToInt32(numbers.NumberOfCases) + 1).ToString(), Time = DateTime.Now.ToString("MM/dd/yyyy") });


        }
        public static async Task AddVirusCaseInfo(string role, string name, string school, string city, string NewUserEmail, string viruscase, string classcode)
        {
            string CitySchool = city.ToLower() + school.ToLower();

            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

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
             .Child(CitySchool.Replace(" ", ""))
             .Child("NurseInfo")
             .Child(person.UID)
             .Child("HealthInfoVirusCases")
             .PostAsync(new Person() { NewUserEmail = NewUserEmail, Role = role, Name = name, ClassCode = classcode.ToLower(), HealthInfoVirusCase = viruscase, Time = DateTime.Now.ToString("MM/dd/yyyy") });

                
            }


        }
        public static async Task AddEnteredInfoForVirusCases(string viruscase)
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();
            await firebaseClient
            .Child("PersonEnteredInfo")
            .Child(user.LocalId)
            .PostAsync(new Person() { Time = DateTime.Now.ToString("MM/dd/yyyy"), Notification = "Virus Case Entered: " + viruscase });

        }
        public static async Task SendNurseNotificationsForVirusCase(string city, string school, string classcode)
        {
            string CitySchool = city.ToLower() + school.ToLower();

            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();
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
            foreach(var person in listofpeople)
            {
                await firebaseClient
                    .Child(CitySchool.Replace(" ", ""))
                    .Child("Notifications")
                    .Child(person.UID)
                    .PostAsync(new Person() { Notification = "Virus case in " + classcode, NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });
            }
            
        }
        public static async Task MarkAsRead(string city, string school)
        {
            string CitySchool = city.ToLower() + school.ToLower();
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();


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

        
        public static async Task AddAllergyNurseNotifications(string name, string role, string school, string city, string classcode, string foodallergies, string email, string allergydetails, string permission, string parentname)
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

            string CitySchool = city.ToLower() + school.ToLower();

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

        public static async Task AddAllergy(string name, string role, string school, string city, string classcode, string foodallergies, string email, string allergydetails, string permission, string parentname)
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

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
                  .ToList().Where(a=>a.UID!=user.LocalId).Where(a=>a.UID != schoolnurse[0].UID);

                foreach (var Uid in peopleinclass)
                {
                    await firebaseClient
                  .Child(CitySchool.ToLower().Replace(" ", ""))
                  .Child("Notifications")
                  .Child(Uid.UID)
                  .PostAsync(new Person() { Notification = "Allergy in " + classcode + ": " + foodallergies + " (" + allergydetails + ")", NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });
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

                


            }
            else
            {
                if (role == "Teacher")
                {
                    await firebaseClient
              .Child(CitySchool.Replace(" ", ""))
              .Child("TeacherInfo")
              .Child(classcode.ToLower())
              .Child(teacher.UID)
              .Child("FoodAllergies")
              .PostAsync(new Person() { FoodAllergies = foodallergies, AllergyDetails = allergydetails, Name = name, NewUserEmail = email, ClassCode = classcode });

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
                    await firebaseClient
              .Child(CitySchool.ToLower().Replace(" ", ""))
              .Child("Notifications")
              .Child(teacher.UID)
              .PostAsync(new Person() { Notification = "Allergy: " + foodallergies + " (" + allergydetails + ")", NotificationStatus = "Unread", Time = DateTime.Now.ToString("MM/dd/yyyy") });
                }

                
            }

         
        }
        public static async Task<List<Person>> GetAllPeopleFromClassCode(string school, string city)
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

            var CitySchool = city + school;
            var list = await FirebaseHelper.GetPersonsClassCodes();
            var people = new List<Person>();
            foreach(var classcode in list)
            {
                var peopleinclass = (await firebaseClient
                  .Child(CitySchool.Replace(" ", "").ToLower())
                  .Child("ClassInfo")
                  .Child(classcode.ToLower())
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      Name = item.Object.Name,
                      ClassCode = classcode

                  })
                  .ToList();
                foreach(var person in peopleinclass)
                {
                    people.Add(person);
                }
            }
            
            return people;
        }

        public static async Task AddMedicalAllergyStudent(string name, string role, string StudentName, string school, string city, string classcode, string medicalallergy, string email)
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

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

                
            }




        }
        public static async Task AddMedicalAllergyNurseNotification(string classcode, string city, string school)
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();
            string CitySchool = city.ToLower() + school.ToLower();
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
                  .Where(a => a.Role == "Nurse").Where(a => (a.City + a.School).ToLower().Replace(" ", "") == (CitySchool.Replace(" ", "")))
                  .ToList();
            foreach (var person in schoolnurse)
            {
                await firebaseClient
                .Child(CitySchool.Replace(" ", ""))
                .Child("Notifications")
                .Child(person.UID)
                .PostAsync(new Person() { Notification = "Medical Allergy Entered in " + classcode, Time = DateTime.Now.ToString("MM/dd/yyyy"), NotificationStatus = "Unread" });
            }
            }
        public static async Task AddNursePastEnteredInfo(string email, string category, string info)
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();
            await firebaseClient.Child("PersonEnteredInfo").Child(user.LocalId).PostAsync(new Person() { Notification = category + " Entered: " + info + " (" + email + ")" , Time = DateTime.Now.ToString("MM/dd/yyyy") });
        }
        public static async Task AddAllergyNurseNotifications2(string classcode, string city, string school, string foodallergies)
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();
            string CitySchool = city.ToLower() + school.ToLower();
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
                  .Where(a => a.Role == "Nurse").Where(a => (a.City + a.School).ToLower().Replace(" ", "") == (CitySchool.Replace(" ", "")))
                  .ToList();
            foreach (var person in schoolnurse)
            {
                await firebaseClient
                .Child(CitySchool.Replace(" ", ""))
                .Child("Notifications")
                .Child(person.UID)
                .PostAsync(new Person() { Notification = "Food Allergy Entered in " + classcode, Time = DateTime.Now.ToString("MM/dd/yyyy"), NotificationStatus = "Unread" });
            }
            await firebaseClient
     .Child("PersonEnteredInfo")
     .Child(user.LocalId)
     .PostAsync(new Person() { Time = DateTime.Now.ToString("MM/dd/yyyy"), Notification = "Allergy Entered: " + foodallergies });


        }
        public static async Task AddMedicalAllergyTeacher(string name, string role, string school, string city, string classcode, string medicalallergy, string email)
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

            string CitySchool = city.ToLower() + school.ToLower();
            var schoolnurse = (await firebaseClient
                  .Child("Persons")
                  .OnceAsync<Person>())
                  .Select(item => new Person
                  {
                      City = item.Object.City.ToString(),
                      School = item.Object.School,
                      Role = item.Object.Role.ToString(),
                      UID = item.Key.ToString()

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
                .PostAsync(new Person() { Notification = "Medical Allergy Entered in " + classcode, Time = DateTime.Now.ToString("MM/dd/yyyy"), NotificationStatus="Unread" });
            }



        }


        public static async Task AddClass(string classcode, string name, string school, string city)
        {
            var user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();
            
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
        .Child("ClassInfo")
        .Child(classcode.ToLower())
        .Child(person.UID)
        .PutAsync(new Person() { Name = name });

                await firebaseClient
            .Child(CitySchool.Replace(" ", ""))
            .Child("Classes")
            .Child(person.UID)
            .PostAsync(new Person() { ClassCode = classcode });
              
            }
        }
        public static async Task AddToClass(string classcode, string school, string name, string city)
        {
            Firebase.Auth.User user = await FirebaseHelper.GetUser();

            var firebaseClient = await FirebaseHelper.GetFirebaseClientUser();

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

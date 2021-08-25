using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SampleDataGenerator
{
    public class SampleCustomer
    {
        [JsonProperty(PropertyName = "myPartitionKey")]
        public string MyPartitionKey { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "postalcode")]
        public string PostalCode { get; set; }

        [JsonProperty(PropertyName = "region")]
        public string Region { get; set; }

        [JsonProperty(PropertyName = "userDefinedId")]
        public int UserDefinedId { get; set; }

        [JsonProperty(PropertyName = "wordsArray")]
        public string[] WordsArray { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public Tags Tags { get; set; }

        [JsonProperty(PropertyName = "recipientList")]
        public ArrayList RecipientList { get; set; }

        public static List<SampleCustomer> GenerateManyCustomers(string partitionKeyValue, int number)
        {
            Tags tags = new Tags();
            ArrayList RecipientList = new ArrayList();
            int numberOfAddressInfo = 0;

            //Generate fake customer data.
            Bogus.Faker<SampleCustomer> customerGenerator = new Bogus.Faker<SampleCustomer>().Rules((faker, customer) =>
            {
                customer.Id = Guid.NewGuid().ToString();
                customer.Name = faker.Name.FullName();
                customer.City = faker.Person.Address.City.ToString();
                customer.Region = faker.Person.Address.State.ToString();
                customer.PostalCode = faker.Person.Address.ZipCode.ToString();
                customer.MyPartitionKey = customer.Region;
                customer.UserDefinedId = faker.Random.Int(0, 1000);
                customer.WordsArray = faker.Random.WordsArray(1, 5);

                tags.Words = faker.Random.WordsArray(1, 3);
                tags.Numbers = faker.Random.AlphaNumeric(250);

                customer.Tags = tags;
                RecipientList.Clear();
                int randomInt = 0;
                customer.RecipientList = RecipientList;

                randomInt = faker.Random.Int(0, 100);

                numberOfAddressInfo = randomInt + numberOfAddressInfo;

                int counter = 0;

                while (counter < randomInt)
                {
                    AddressInfo addressInfo = new AddressInfo();

                    addressInfo.Name = faker.Name.FullName();
                    addressInfo.City = faker.Person.Address.City;
                    addressInfo.Region = faker.Person.Address.State;
                    addressInfo.PostalCode = faker.Person.Address.ZipCode;
                    addressInfo.Quantity = faker.Random.Int(0, 1000);
                    addressInfo.Guid = Guid.NewGuid().ToString();

                    customer.RecipientList.Add(addressInfo);
                    counter++;
                }
            });

            return customerGenerator.Generate(number);
        }


    }
    public class Tags
    {
        [JsonProperty(PropertyName = "words")]
        public string[] Words { get; set; }
        [JsonProperty(PropertyName = "numbers")]
        public string Numbers { get; set; }
        public Tags()
        {

        }
    }

    public class AddressInfo
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "postalcode")]
        public string PostalCode { get; set; }

        [JsonProperty(PropertyName = "region")]
        public string Region { get; set; }

        [JsonProperty(PropertyName = "guid")]
        public string Guid { get; set; }

        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }
    }
}
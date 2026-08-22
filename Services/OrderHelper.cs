namespace BestStoreAPI.Services
{
    public class OrderHelper
    {
        public static decimal shippingFee { get; } = 5;
        public static Dictionary<string, string> PaymentMethods { get; }  = new ()
        {
            {"paypal", "PayPal"
            },
            {
                "creditcard", "Credit Card"
            },
            {
                "banktransfer", "Bank Transfer"
            }   

         };

        public static List<string> PaymentStatuses { get; } = new ()
        {
            "pending",
            "Accepted",
            "Canceled"
        };
        public static List<string> OrderStatuses { get; } = new ()
        {
            "Created",
            "Accepted",
            "Canceled",
            "Shipped",
            "Delivered",
            "returned"
        };

        public static Dictionary<int , int> GetProductDictionary(string productIdentifiers)
        {
            var productDictionary = new Dictionary<int, int>();
            if (productIdentifiers.Length > 0 )
            {
                string[] productIdArray = productIdentifiers.Split('-');
                foreach(var productId in productIdArray)
                {
                    try
                    {
                        int id = int.Parse(productId);
                        if (productDictionary.ContainsKey(id))
                        {
                            productDictionary[id]++;
                        }
                        else
                        {
                            productDictionary.Add(id, 1);
                        }

                    }
                    catch (Exception ex) { }
                    
                }

            }
            return productDictionary;


        }
        
    }
}

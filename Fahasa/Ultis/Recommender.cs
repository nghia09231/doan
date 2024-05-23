using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Ultis
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public List<int> PurchasedProducts { get; set; }

        public Customer() {
            CustomerId = -100;
            PurchasedProducts = new List<int>();
        }
    }
    public class Recommender
    {
        public static List<int> GetRecommendedProducts(List<Customer> customers, int targetCustomerId)
        {
            // Tính toán khoảng cách giữa khách hàng mục tiêu và tất cả các khách hàng khác
            Dictionary<int, double> distances = new Dictionary<int, double>();

            Customer targetCustomer = customers.Find(c => c.CustomerId == targetCustomerId);

            foreach (Customer customer in customers)
            {
                if (customer.CustomerId != targetCustomerId)
                {
                    double distance = CalculateDistance(targetCustomer, customer);
                    distances.Add(customer.CustomerId, distance);
                }
            }

            // Sắp xếp theo khoảng cách để tìm các khách hàng tương tự nhất
            List<int> similarCustomers = distances.OrderBy(x => x.Value).Select(x => x.Key).ToList();

            // Tạo danh sách sản phẩm gợi ý dựa trên lịch sử mua hàng của khách hàng tương tự
            List<int> recommendedProducts = new List<int>();

            foreach (int customer in similarCustomers)
            {
                Customer similarCustomer = customers.Find(c => c.CustomerId == customer);
                if (recommendedProducts.Count > 5)
                {
                    break;
                }
                foreach (int product in similarCustomer.PurchasedProducts)
                {
                    if (recommendedProducts.Count > 5)
                    {
                        break;
                    }
                    if (!targetCustomer.PurchasedProducts.Contains(product) && !recommendedProducts.Contains(product))
                    {
                        recommendedProducts.Add(product);
                    }
                }
            }
            
            return recommendedProducts;
        }

        static double CalculateDistance(Customer customer1, Customer customer2)
        {
            // Trong ví dụ này, bạn có thể tính khoảng cách bằng cách đếm số sản phẩm chung và chưa chung giữa hai khách hàng.

            List<int> products1 = customer1.PurchasedProducts;
            List<int> products2 = customer2.PurchasedProducts;

            int commonProducts = products1.Intersect(products2).Count();
            int allProducts = products1.Union(products2).Count();

            return 1 - (double)commonProducts / allProducts;
        }
    }
}
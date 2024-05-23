using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Fahasa.Models;
using WebGrease.Css.Extensions;

namespace Fahasa.Ultis
{
    public class MailUltis
    {
        public static async Task<bool> SendMail(string _from, string _to, string _subject, string _body, SmtpClient client)
        {
            MailMessage message = new MailMessage(
                from: _from,
                to: _to,
                subject: _subject,
                body: _body
            );
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            message.ReplyToList.Add(new MailAddress(_from));
            message.Sender = new MailAddress(_from);


            try
            {
                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static async Task<bool> SendMailGoogleSmtp(string _from, string _to, string _subject, string _body, string _gmailsend, string _gmailpassword)
        {

            MailMessage message = new MailMessage(
                from: _from,
                to: _to,
                subject: _subject,
                body: _body
            );
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            message.ReplyToList.Add(new MailAddress(_from));
            message.Sender = new MailAddress(_from);

            using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                client.Port = 587;
                client.Credentials = new NetworkCredential(_gmailsend, _gmailpassword);
                client.EnableSsl = true;
                return await SendMail(_from, _to, _subject, _body, client);
            }

        }

        public static string getBodyMailConfirmCheckout(Order order) {
			string listOrderDetails = "";
			string payment = "";

			switch (order.PaymentMethod) {
				case "cashondelivery":
					payment = "Thanh toán khi nhận hàng";
					break;

            }

            var preCart = Fahasa.Ultis.CartUltis.calcOrder(((long)order.OrderDetails.Sum(x => x.Price * x.Amount)), order.Discounts.ToList(), order.FeeShip ?? 0);
            order.OrderDetails.ForEach(o =>
			{
				listOrderDetails += @"<tr>
							<td style='width: 40%'>
								<div style='padding-right: 10px'>
									<a
										href='/Products/Details/" + o.Product.Id + @"'
										target='_blank'><img
											src='" + o.Product.ImageSrc + @"'
											style='width: 100%; max-width: 160px'
											class='CToWUd'
											data-bit='iit'
									/></a>
								</div>
							</td>
							<td style='width: 60%'>
								<div style='margin-bottom: 8px'>
									<a
										style='
											color: #173948 !important;
											font-size: 14px !important;
											text-decoration: none !important;
										'
										href='/Products/Details/" + o.Product.Id + @"'
										target='_blank'
										><span style='font-size: 14px'
											>
											" + o.Product.Name + @"
											</span
										></a
									>
								</div>
								<div style='margin-bottom: 8px'>
									<span style='font-size: 18px; color: c92127'>" + PriceUltis.getShow(((long)o.Price)) + @" đ</span>
								</div>
								<div>
									<span style='font-size: 14px'>Số lượng: " + o.Amount + @"</span>
								</div>
							</td>
						</tr>";
			});

			return @"
                <div style='width: 100%; max-width: 750px; margin: auto'>
	<div style='padding-top: 0px; padding: 30px; background: #fff; border-bottom: 10px solid #f0f0f0'>
		<div class='' style='color: #0f146d; text-align: center'>
			Cám ơn bạn đã đặt hàng tại <span class='il'>Fahasa</span>!
		</div>
		<div class=''>
			<h2>Xin chào " + order.Person.Name + @"</h2>

			<p>
				<span class='il'>Fahasa</span> đã nhận được yêu cầu đặt hàng của bạn và đang xử lý nhé. Bạn sẽ nhận được
				thông báo tiếp theo khi đơn hàng đã sẵn sàng được giao.
			</p>

			<div class='m_6310239507822873873two_col' align='center' style='padding-top: 10px'>
				<div>
					<a
						href='https://tracking.ghn.dev/?order_code="+order.ShipCode+@"'
						target='_blank'
					>
						<img
							src='https://ci4.googleusercontent.com/proxy/hb83_q3VEXZ-Uwy_xppTnb1XLAclbqMzS9O7MrdSX5vaWwgQ89wyJCDqMR_QFshTJmVyMN9u9VRKbJh1OzYtBskvELQaLcbZNZRYk4Pzxd2sYPIX=s0-d-e1-ft#https://img.alicdn.com/tfs/TB178CsJeH2gK0jSZFEXXcqMpXa-300-50.jpg'
							style='max-width: 300px'
							border='0'
							class='CToWUd'
							data-bit='iit'
						/>
					</a>
				</div>
			</div>

			<p>
				<b>*Lưu ý nhỏ cho bạn:</b> Bạn chỉ nên nhận hàng khi trạng thái đơn hàng là “<b>Đang giao hàng</b>” và
				nhớ kiểm tra Mã đơn hàng, Thông tin người gửi và Mã vận đơn để nhận đúng kiện hàng nhé.
			</p>
		</div>
	</div>
	<div style='padding: 30px; background: #fff; border-bottom: 10px solid #f0f0f0'>
		<div class='m_-4420525363731220642section-header m_-4420525363731220642section-header--deliveredTo'>
			Đơn hàng được giao đến
		</div>
		<div class='m_-4420525363731220642section-content'>
			<table cellpadding='2' cellspacing='0' width='100%'>
				<tbody>
					<tr>
						<td width='25%' valign='top' style='color: #0f146d; font-weight: bold'>Tên:</td>
						<td width='75%' valign='top'>" + order.Person.Name + @"</td>
					</tr>
					<tr>
						<td valign='top' style='color: #0f146d; font-weight: bold'>Địa chỉ nhà:</td>
						<td valign='top'>Quận Tân Phú, Hồ Chí Minh, Phường Tây Thạnh, Đường D11 -</td>
					</tr>
					<tr>
						<td valign='top' style='color: #0f146d; font-weight: bold'>Điện thoại:</td>
						<td valign='top'>" + order.Person.Phone + @"</td>
					</tr>
					<tr>
						<td valign='top' style='color: #0f146d; font-weight: bold'>Email:</td>
						<td valign='top'>
							<a href='mailto:letranglan129@gmail.com' target='_blank'>" + order.Person.Email + @"</a>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</div>

	<div style='padding-bottom: 0px; padding: 30px; background: #fff; border-bottom: 10px solid #f0f0f0'>
		<div class='m_-4420525363731220642section-content'>
			<div class='m_-4420525363731220642section-header m_-4420525363731220642section-header--yourPackage'>
				Kiện Hàng
			</div>
			<div class='m_-4420525363731220642product' style='border-bottom: 0px none'>
				<table cellpadding='0' cellspacing='0' style='width: 100%'>
					<tbody>
						"+ listOrderDetails + @"
					</tbody>
				</table>
			</div>
		</div>
	</div>

	<div style='padding-top: 0px; padding: 30px; background: #fff; border-bottom: 10px solid #f0f0f0'>
		<div style='width: 100%'>
			<div style='display: inline-block; width: 100%; margin-top: 20px'>
				<div style='width: 100% !important; float: none !important; max-width: none !important'>
					<table
						cellpadding='0'
						cellspacing='0'
						class='m_-4420525363731220642checkout-amount'
						style='border-bottom: 1px solid #d8d8d8; width: 100%; line-height: 24px'
					>
						<tbody>
							<tr>
								<td valign='top' style='color: #585858; width: 49%; height: 35px;'>Thành tiền:</td>
								<td align='right' valign='top'>VND</td>
								<td align='right' valign='top'>"+ PriceUltis.getShow(preCart.total)  + @"</td>
							</tr>
							<tr>
								<td valign='top' style='color: #585858; height: 35px;'>Phí vận chuyển:</td>
								<td align='right' valign='top'>VND</td>
								<td align='right' valign='top'>"+ Fahasa.Ultis.PriceUltis.getShow(order.FeeShip) + @"</td>
							</tr>
							<tr>
								<td valign='top' style='color: #585858; height: 35px;'>Giảm giá:</td>
								<td align='right' valign='top'>VND</td>
								<td align='right' valign='top'>"+ Fahasa.Ultis.PriceUltis.getShow(preCart.sumDiscountValue) + @"</td>
							</tr>
							<tr>
								<td valign='top' style='color: #585858; height: 35px;'>Tổng cộng:</td>
								<td align='right' valign='top'>
									<div style='color: #f27c24; font-weight: bold'>VND</div>
								</td>
								<td align='right' valign='top'>
									<div style='color: #f27c24; font-weight: bold'>"+ Fahasa.Ultis.PriceUltis.getShow(preCart.totalPaid)+@"</div>
								</td>
							</tr>
						</tbody>
					</table>
					<br />

					<table cellpadding='0' cellspacing='0' style=' width: 100%; line-height: 24px'>
						<tbody>
							<tr>
								<td valign='top' style='color: #585858; width: 49%'>Hình thức thanh toán:</td>
								<td align='right' valign='top' colspan='2'>"+ payment + @"</td>
							</tr>
						</tbody>
					</table>
				</div>
			</div>
		</div>
	</div>
</div>

";
        }

        public static string getBodyMailCancelOrder(Order order)
        {
            string listOrderDetails = "";
            string payment = "";

            switch (order.PaymentMethod)
            {
                case "cashondelivery":
                    payment = "Thanh toán khi nhận hàng";
                    break;

            }

            var preCart = Fahasa.Ultis.CartUltis.calcOrder(((long)order.OrderDetails.Sum(x => x.Price * x.Amount)), order.Discounts.ToList(), order.FeeShip ?? 0);
            order.OrderDetails.ForEach(o =>
            {
                listOrderDetails += @"<tr>
							<td style='width: 40%'>
								<div style='padding-right: 10px'>
									<a
										href='/Products/Details/" + o.Product.Id + @"'
										target='_blank'><img
											src='" + o.Product.ImageSrc + @"'
											style='width: 100%; max-width: 160px'
											class='CToWUd'
											data-bit='iit'
									/></a>
								</div>
							</td>
							<td style='width: 60%'>
								<div style='margin-bottom: 8px'>
									<a
										style='
											color: #173948 !important;
											font-size: 14px !important;
											text-decoration: none !important;
										'
										href='/Products/Details/" + o.Product.Id + @"'
										target='_blank'
										><span style='font-size: 14px'
											>
											" + o.Product.Name + @"
											</span
										></a
									>
								</div>
								<div style='margin-bottom: 8px'>
									<span style='font-size: 18px; color: c92127'>" + PriceUltis.getShow(((long)o.Price)) + @" đ</span>
								</div>
								<div>
									<span style='font-size: 14px'>Số lượng: " + o.Amount + @"</span>
								</div>
							</td>
						</tr>";
            });

            return @"
                <div style='width: 100%; max-width: 750px; margin: auto'>
	<div style='padding-top: 0px; padding: 30px; background: #fff; border-bottom: 10px solid #f0f0f0'>
		<div class='' style='color: #0f146d; text-align: center;font-size: 23px;'>
			Đơn hàng của bạn đã được hủy
		</div>
		<div class=''>
			<h2>Xin chào " + order.Person.Name + @"</h2>

			<p>
				<span class='il'>Fahasa</span> đã nhận được yêu cầu hủy đơn đặt hàng của bạn
			</p>
			<p>
				Lazada mong bạn sớm tìm được sản phẩm phù hợp để tiếp đồng hành cùng Lazada nhé!
			</p>
		</div>
	</div>
	
	<div style='padding-bottom: 0px; padding: 30px; background: #fff; border-bottom: 10px solid #f0f0f0'>
		<div class='m_-4420525363731220642section-content'>
			<div class='m_-4420525363731220642section-header m_-4420525363731220642section-header--yourPackage'>
				Kiện Hàng
			</div>
			<div class='m_-4420525363731220642product' style='border-bottom: 0px none'>
				<table cellpadding='0' cellspacing='0' style='width: 100%'>
					<tbody>
						" + listOrderDetails + @"
					</tbody>
				</table>
			</div>
		</div>
	</div>

	<div style='padding-top: 0px; padding: 30px; background: #fff; border-bottom: 10px solid #f0f0f0'>
		<div style='width: 100%'>
			<div style='display: inline-block; width: 100%; margin-top: 20px'>
				<div style='width: 100% !important; float: none !important; max-width: none !important'>
					<table
						cellpadding='0'
						cellspacing='0'
						class='m_-4420525363731220642checkout-amount'
						style='border-bottom: 1px solid #d8d8d8; width: 100%; line-height: 24px'
					>
						<tbody>
							<tr>
								<td valign='top' style='color: #585858; width: 49%; height: 35px;'>Thành tiền:</td>
								<td align='right' valign='top'>VND</td>
								<td align='right' valign='top'>" + PriceUltis.getShow(preCart.total) + @"</td>
							</tr>
							<tr>
								<td valign='top' style='color: #585858; height: 35px;'>Phí vận chuyển:</td>
								<td align='right' valign='top'>VND</td>
								<td align='right' valign='top'>" + Fahasa.Ultis.PriceUltis.getShow(order.FeeShip) + @"</td>
							</tr>
							<tr>
								<td valign='top' style='color: #585858; height: 35px;'>Giảm giá:</td>
								<td align='right' valign='top'>VND</td>
								<td align='right' valign='top'>" + Fahasa.Ultis.PriceUltis.getShow(preCart.sumDiscountValue) + @"</td>
							</tr>
							<tr>
								<td valign='top' style='color: #585858; height: 35px;'>Tổng cộng:</td>
								<td align='right' valign='top'>
									<div style='color: #f27c24; font-weight: bold'>VND</div>
								</td>
								<td align='right' valign='top'>
									<div style='color: #f27c24; font-weight: bold'>" + Fahasa.Ultis.PriceUltis.getShow(preCart.totalPaid) + @"</div>
								</td>
							</tr>
						</tbody>
					</table>
					<br />

					<table cellpadding='0' cellspacing='0' style=' width: 100%; line-height: 24px'>
						<tbody>
							<tr>
								<td valign='top' style='color: #585858; width: 49%'>Hình thức thanh toán:</td>
								<td align='right' valign='top' colspan='2'>" + payment + @"</td>
							</tr>
						</tbody>
					</table>
				</div>
			</div>
		</div>
	</div>
</div>

";
        }
    }
}
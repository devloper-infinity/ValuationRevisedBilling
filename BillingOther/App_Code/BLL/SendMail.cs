using BillingOther.App_Code.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace BillingOther.App_Code.BLL
{
    public class SendMail
    {
        string Header = "<html><head><meta content='text/html; charset=utf-8' http-equiv='Content-Type'><title></title><style type='text/css'>a:hover { text-decoration: none !important; }.header h1 {color: #fff !important; font: normal 33px Georgia, serif; margin: 0; padding: 0; line-height: 33px;}.header p {color: #dfa575; font: normal 11px Georgia, serif; margin: 0; padding: 0; line-height: 11px; letter-spacing: 2px}.content h2 {color:#8598a3 !important; font-weight: normal; margin: 0; padding: 0; font-style: italic; line-height: 30px; font-size: 30px;font-family: Georgia, serif; }.content p {color:#767676; font-weight: normal; margin: 0; padding: 0; line-height: 20px; font-size: 12px;font-family: Georgia, serif;}.content a {color: #d18648; text-decoration: none;}.footer p {padding: 0; font-size: 11px; color:#fff; margin: 0; font-family: Georgia, serif;}.footer a {color: #f7a766; text-decoration: none;}</style></head><body><table cellpadding='0' cellspacing='0' border='1'><tr><td ><table cellpadding='0' cellspacing='0' border='0' align='center' width='100%' style='font-family: Georgia, serif;' class='header'><tr><td bgcolor='#16a085' height='70' align='center'><h1 style='color: #fff; font: normal 25px Verdana; margin: 0; padding: 0; line-height: 33px;'>Infinity Invoice</h1></td></tr><tr><td style='font-size: 1px; height: 5px; line-height: 1px;' height='5'>&nbsp;</td></tr></table>";

        string Footer = "<table cellpadding='0' cellspacing='0' border='0' align='center' width='100%' style='font-family: Georgia, serif; line-height: 10px; margin-top:30px;' bgcolor='#16a085' class='footer'><tr><td bgcolor='#16a085'  align='center' style='padding: 15px 0 10px; font-size: 11px; color:#fff; margin: 0; line-height: 1.2;font-family: Verdana;' valign='top'><p style='padding: 0; font-size: 11px; color:#fff; margin: 0; font-family: Georgia, serif;'>!!! This is software generated e-mail...Please do not reply. !!</p></td></tr> </table></td></tr></table></body></html>";
        bllTracking blltracking = new bllTracking();

        public void SendProductionData(string ProjectName, string BillingPeriod, string Remark)
        {
            string ToAddress = "";
            string ToCC = "";
            string ToBCC = "";

            StringBuilder htmlBody = new StringBuilder();
            htmlBody.Append("<table width=\"100%\"><tr><td align=\"left\"><b>Dear Sir/Madam,</b></td></tr><tr>");
            htmlBody.Append("<td align=\"left\">This is to inform you that billing data for project <b>" + ProjectName + "</b> and Billing Period <b>" + BillingPeriod + " </b>has been send back to production with below remark from Billing Team.<br /><br /> <span style='color:brown; font-size:14px;'>" + Remark + "</span> </td></tr></table><br />");
            htmlBody.Append("</br><hr /><table width=\"100%x\"><p><b><font size=1>CONFIDENTIALITY INFORMATION AND DISCLAIMER:</font></b><br><font size=1 face=Verdana>This message contains information which may be confidential and privileged. Unless you are the addressee (or authorized to receive for the addressee), you may not use copy or disclose to anyone the message or any information contained in the message. If you have received the message in error, please advise the sender by reply e-mail and delete the message. Thank you...!!!</font></p><br><p><b>Note: This is a software generated mail. Please do not reply.</b></p></font></p></body></html>");
            string path = "E:/EmailPages/SendBackToProduction_" + ProjectName + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".html";
            FileStream fp = File.Create(path);
            fp.Close();
            using (StreamWriter w = new StreamWriter(path, true))
            {
                w.WriteLine(Header + htmlBody + Footer); // Write the text
            }
            string Subject = ProjectName + " - Send back to production";
            ToAddress = "n.nilkanth@infinityinternationals.us"; //alpana@infinitytitleinfo.com
            ToCC = "";
            ToBCC = "n.nilkanth@infinityinternationals.us";
            if (ToAddress == "")
            {
                ToAddress = "n.nilkanth@infinityinternationals.us";
            }

            Hashtable htParam = new Hashtable();
            htParam.Add("EmailType", "");
            htParam.Add("To", Convert.ToString(ToAddress));
            htParam.Add("CC", ToCC);
            htParam.Add("BCC", ToBCC);
            htParam.Add("Subject", Subject);
            htParam.Add("Body", path);
            htParam.Add("Attachment", "");
            htParam.Add("DraftNo", "1");
            int ReturnValue = InsertAutoEmailTracking(htParam);

        }
        public bool sendMail(string EmailType, string Subject, StringBuilder htmlBody, string strEmailTo)
        {
            string ToAddress = string.Empty;
            string ToCC = string.Empty;
            string ToBCC = string.Empty;


            String Body = htmlBody.ToString();
            StringBuilder template = new StringBuilder();
            template.Append("<html><head></head><body>");
            template.Append(Body);
            template.Append("</body></html>");
            MailMessage mail = new MailMessage();
            if (strEmailTo != "")
                ToAddress = ToAddress + ',' + strEmailTo;
            mail.To.Add("anita@infinity-data.com.pointofmail.com");
            //mail.To.Add(ToAddress);
            if (ToCC != "")
                mail.CC.Add(ToCC);
            if (ToBCC != "")
                mail.Bcc.Add(ToBCC);
            //mail.Bcc.Add("n.nilkanth@infinityinternationals.us");
            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure | DeliveryNotificationOptions.OnSuccess | DeliveryNotificationOptions.Delay;
            //Add "Disposition-Notification-To" for Read receipt
            mail.Headers.Add("Disposition-Notification-To", "n.nilkanth@infinityinternationals.us");
            //mail.Headers.Add("Return-Receipt-To", "n.nilkanth@infinityinternationals.us");
            //mail.From = new MailAddress("ack@infinityinternationals.us", "Infinity Billing", System.Text.Encoding.UTF8);
            mail.From = new MailAddress("AR@infinity-data.com", "Infinity Billing", System.Text.Encoding.UTF8);
            mail.Subject = Subject;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = template.ToString();
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = System.Net.Mail.MailPriority.High;

            //SmtpClient client = new SmtpClient();
            //client.Credentials = new System.Net.NetworkCredential("ack@infinityinternationals.us", "Inf1n1ty");

            ////mail.Attachments.Add(new Attachment(System.Web.HttpContext.Current.Server.MapPath("~/ILSDocuments/ILS_UserManual.docx")));
            //client.Host = "109.199.96.191";

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("AR@infinity-data.com", "$of+@123");
            //client.Credentials = new System.Net.NetworkCredential("ack@infinityinternationals.us", "Inf1n1ty");
            client.Port = 587;
            client.Host = "smtp.office365.com";
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            try
            {
                client.Send(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void SendInvoiceCreationEmail(int InvoiceID, int AddedBy, int ClientID)
        {
            StringBuilder htmlBody = new StringBuilder();
            StringBuilder htmlBody_Service = new StringBuilder();
            string ToAddress = "";
            string ToCC = "";
            string sub = "";
            string ToBCC = "";
            //string ToBCC = "s.swapnali@infinityinternationals.us";

            DataTable dt = new bllLogin().GetUserInformation(AddedBy);

            DataTable DTClientDetails = new bllTracking().GetClientDetails(ClientID);

            DataTable DtInvoice = blltracking.GetInvoiceDetails(InvoiceID);
            // string ClientEmail = Convert.ToString(DTClientDetails.Rows[0]["EmailID"]);


            htmlBody_Service.Append("<table width=\"550px\" ><tr><td align=\"left\"><b>Dear Sir/Madam,</b></td></tr><tr>");
            htmlBody_Service.Append("<td align=\"left\">Please find attached herewith invoice #  <b>" + Convert.ToString(DtInvoice.Rows[0]["InvoiceNumber"])
                + "</b>  has been generated successfully.Please check and approve the same.</td></tr></table><br /><table cellspacing='7px' cellpadding='3px' width='400px' style=' border-collapse: collapse;' border='1'\"><tr>");
            htmlBody_Service.Append("<td align=\"left\">Invoice Number : </td><td align=\"bottom\">" + Convert.ToString(DtInvoice.Rows[0]["InvoiceNumber"]) + "</td></tr><tr>");
            htmlBody_Service.Append("<td align=\"left\">Project Number : </td><td align=\"bottom\">" + Convert.ToString(DtInvoice.Rows[0]["ProjectNumber"]) + "</td></tr><tr>");
            htmlBody_Service.Append("<td align=\"left\">Billing Period : </td><td align=\"bottom\">" + Convert.ToString(DtInvoice.Rows[0]["BillingPeriod"]) + "</td></tr></table>");

            htmlBody_Service.Append("<br /><br /><table width=\"650px\"><tr><td align=\"left\">Thanks,<br />Infinity HRMS</td></tr> </table>");
            //sendMailInvoice("New Invoice Created", "New Invoice  " + Convert.ToString(htProfile["Invoiceno"]) + " has been created.", htmlBody, Convert.ToString(dt.Rows[0]["OfficialEmailID"]));
            string path = "E:/EmailPages/HRMS_BillingInvoiceGeneration_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".html";
            FileStream fp = File.Create(path);
            fp.Close();
            using (StreamWriter w = new StreamWriter(path, true))
            {
                w.WriteLine(Header + htmlBody_Service + Footer); // Write the text
            }
            string Subject = "New Invoice for Billing " + Convert.ToString(DtInvoice.Rows[0]["InvoiceNumber"]);// +" - " + Convert.ToString(htProfile["Location"]);
            // ToAddress = ClientEmail; //Convert.ToString(dt.Rows[0]["OfficialEmailID"]);
            ToAddress = "anita@infinity-data.com";
            ToCC = "";
            ToBCC = "n.nilkanth@infinityinternationals.us";
            if (ToAddress == "")
            {
                ToAddress = "Soft-Team@infinityinternationals.us";
            }

            string Attachment = System.Web.HttpContext.Current.Server.MapPath(DtInvoice.Rows[0]["InvoiceAttachmentsPDF"].ToString());

            Hashtable htParam = new Hashtable();
            htParam.Add("EmailType", "");
            htParam.Add("To", Convert.ToString(ToAddress));
            htParam.Add("CC", ToCC);
            htParam.Add("BCC", ToBCC);
            htParam.Add("Subject", Subject);
            htParam.Add("Body", path);
            htParam.Add("Attachment", Attachment);
            htParam.Add("DraftNo", "1");
            int ReturnValue = InsertAutoEmailTracking(htParam);
        }

        public void SendInvoiceCreationEmailForSummaryReport(string Groupname, string BillingPeriod, int ClientId)
        {
            StringBuilder htmlBody = new StringBuilder();
            StringBuilder htmlBody_Service = new StringBuilder();
            string ToAddress = "";
            string ToCC = "";
            string sub = "";
            string ToBCC = "";
            //string ToBCC = "s.swapnali@infinityinternationals.us";

            //DataTable dt = new bllLogin().GetUserInformation(AddedBy);

            DataTable DTClientDetails = new bllTracking().GetSummaryReportInvoice(Groupname, BillingPeriod);
            DataTable dtClientInfo = new bllTracking().GetClientDetails(Groupname, BillingPeriod);
            DataTable DtInvoice = blltracking.GetSummaryReportAttachments(Groupname, BillingPeriod);
            string ClientEmail = Convert.ToString(dtClientInfo.Rows[0]["PAI_Email_Id"]);
            string clientName = "";

            string ToCCs = Convert.ToString(dtClientInfo.Rows[0]["CEC_CC"]);
            try
            {
                if (ToCCs.Contains(","))
                {
                    string[] ToCCStr = ToCCs.Split(',');
                    foreach (string cc in ToCCStr)
                    {
                        if (cc != "")
                        {
                            ToCC += cc + ".pointofmail.com,";
                        }
                    }
                }
                else
                {
                    ToCC = Convert.ToString(dtClientInfo.Rows[0]["CEC_CC"]);
                }
                ToCC = ToCC.Substring(0, ToCC.LastIndexOf(','));
            }
            catch { }
            if (dtClientInfo != null)
            {
                if (dtClientInfo.Rows.Count > 0)
                {
                    clientName = Convert.ToString(dtClientInfo.Rows[0]["PAI_Contact_Person"]);
                }
            }
            try
            {
                clientName = clientName.Substring(0, clientName.IndexOf(" "));
            }
            catch { }

            htmlBody_Service.Append("<table width=\"650px\" style='margin-left:10px;'><tr><td align=\"left\"><b>Hello " + clientName + ",</b></td></tr><tr>");
            htmlBody_Service.Append("<td align=\"left\">Good Morning!! <br /><br />Please find attached invoice# " + Convert.ToString(DTClientDetails.Rows[0]["InvNo"]) + " for your review.");
            htmlBody_Service.Append("<br /><br />Kindly, For all invoices related queries direct email to following.");
            //htmlBody_Service.Append("<br /><br />Just a note, regarding invoice related queries, please deal with only myself and Mahesh");
            htmlBody_Service.Append("<br /><br /><a href='mailTo:anita@Infinity-data.com'>anita@Infinity-data.com</a>");
            htmlBody_Service.Append("<br /><a href='mailTo:jim@Infinity-data.com'>jim@Infinity-data.com</a>");
            htmlBody_Service.Append("</td></tr></table><br /><br /><br /><br /><table width=\"650px\" style='margin-left:10px;'><tr><td align=\"left\">Thanks,<br />Anita Londhe<br />VP Controller<br />Infinity IPS<br /><a href='mailTo:anita@Infinity-data.com'>anita@Infinity-data.com</a><br /><a href='www.infinity-data.com'>www.infinity-data.com</a></td></tr> </table>");

            htmlBody_Service.Append("<table width=\"650px\"><tr><td align=\"left\"></td></tr></table>");
            htmlBody_Service.Append("<br /><span style='color:red; font-size:14px;'>**WE DO NOT ACCEPT OR REQUEST CHANGES TO WIRING INSTRUCTION VIA EMAIL - Always call to verify**</span>");
            htmlBody_Service.Append("<br /><span>**********************************************************************************************************</span>");
            htmlBody_Service.Append("<br /><br /><span style='font-size:11px;'>Disclaimer: The information contained in this e-mail and any attachments may be confidential or privileged under applicable law, or otherwise may be protected from disclosure to anyone other than the intended recipient(s). Any use, distribution, or copying of this e-mail, including any of its contents or attachments by any person other than the intended recipient, or for any purpose other than its intended use, is strictly prohibited. If you believe you have received this e-mail in error, please notify us by e-mail and permanently delete the e-mail and any attachments, and do not save, copy, disclose, or reply on any part of the information contained in this e-mail or its attachments</span>");

            string path = "E:/EmailPages/HRMS_BillingSummaryReportEmail_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".html";
            FileStream fp = File.Create(path);
            fp.Close();
            using (StreamWriter w = new StreamWriter(path, true))
            {
                w.WriteLine(Header + htmlBody_Service + Footer); // Write the text
            }
            string Subject = "Infinity invoice " + Convert.ToString(DtInvoice.Rows[0]["Invoice_GroupNumber"]);
            ToAddress = ClientEmail + ".pointofmail.com"; //Convert.ToString(dt.Rows[0]["OfficialEmailID"]);
            //ToAddress = "anita@infinity-data.com.pointofmail.com";
            //ToCC = "";
            //ToBCC = "n.nilkanth@infinityinternationals.us";
            if (ToAddress == "")
            {
                ToAddress = "anita@infinity-data.com.pointofmail.com";
            }
            //ToCC = "";
            //ToCC = "anita@infinity-data.com";
            ToBCC = "";
            StringBuilder body = new StringBuilder();
            body.Append(Header);
            body.Append(htmlBody_Service);
            body.Append(Footer);
            //sendMail("", Subject, body, "b.shubhangi@infinityinternationals.us");
            //sendMail("", Subject, body, "");

            string Attachment = System.Web.HttpContext.Current.Server.MapPath(DtInvoice.Rows[0]["PGA_AttachmentsPDF"].ToString());

            Hashtable htParam = new Hashtable();
            htParam.Add("EmailType", "");
            htParam.Add("To", Convert.ToString(ToAddress));
            htParam.Add("CC", ToCC);
            htParam.Add("BCC", ToBCC);
            htParam.Add("Subject", Subject);
            htParam.Add("Body", path);
            htParam.Add("Attachment", Attachment);
            htParam.Add("DraftNo", "1");
            int ReturnValue = InsertAutoEmailTracking(htParam);
        }

        public void SendTestEmail(int InvoiceID, int AddedBy, int ClientID, string EmailIds)
        {
            StringBuilder htmlBody = new StringBuilder();
            StringBuilder htmlBody_Service = new StringBuilder();
            string ToAddress = "";
            string ToCC = "";
            string sub = "";
            string ToBCC = "";
            // string ToBCC = "s.swapnali@infinityinternationals.us";

            DataTable dt = new bllLogin().GetUserInformation(AddedBy);

            DataTable DTClientDetails = new bllTracking().GetClientDetails(ClientID);
            DataTable dtEmailDetailsByInvoiceID = new bllTracking().EmailDetailsByInvoiceID(InvoiceID);
            DataTable DtInvoice = blltracking.GetInvoiceDetails(InvoiceID);
            //string ClientEmail = Convert.ToString(DTClientDetails.Rows[0]["EmailID"]);


            htmlBody_Service.Append("<table width=\"550px\" ><tr><td align=\"left\"><b>Dear Sir/Madam,</b></td></tr><tr>");
            htmlBody_Service.Append("<td align=\"left\">Please find attached herewith following invoices.  Kindly confirm <b>"
                + "</b> </td></tr></table><br /><table cellspacing='7px' cellpadding='3px' width='400px' style=' border-collapse: collapse;' border='1'\"><tr></tr></table>");
            //htmlBody_Service.Append("<td align=\"left\">Invoice Number : </td><td align=\"bottom\">" + Convert.ToString(DtInvoice.Rows[0]["InvoiceNumber"]) + "</td></tr><tr>");
            //htmlBody_Service.Append("<td align=\"left\">Project Number : </td><td align=\"bottom\">" + Convert.ToString(DtInvoice.Rows[0]["ProjectNumber"]) + "</td></tr><tr>");
            //htmlBody_Service.Append("<td align=\"left\">Billing Period : </td><td align=\"bottom\">" + Convert.ToString(DtInvoice.Rows[0]["BillingPeriod"]) + "</td></tr></table>");

            htmlBody_Service.Append("<br /><br /><table width=\"650px\"><tr><td align=\"left\">Thanks,<br />Infinity HRMS</td></tr> </table>");
            //sendMailInvoice("New Invoice Created", "New Invoice  " + Convert.ToString(htProfile["Invoiceno"]) + " has been created.", htmlBody, Convert.ToString(dt.Rows[0]["OfficialEmailID"]));
            string path = "E:/EmailPages/HRMS_BillingInvoiceGenerationtestEmail_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".html";
            FileStream fp = File.Create(path);
            fp.Close();
            using (StreamWriter w = new StreamWriter(path, true))
            {
                w.WriteLine(Header + htmlBody_Service + Footer); // Write the text
            }
            string Subject = "Test Invoice Email for Billing " + Convert.ToString(DtInvoice.Rows[0]["InvoiceNumber"]);// +" - " + Convert.ToString(htProfile["Location"]);
            // ToAddress = EmailIds; //Convert.ToString(dt.Rows[0]["OfficialEmailID"]);
            ToAddress = "anita@infinity-data.com.pointofmail.com";
            ToCC = "";
            ToBCC = "n.nilkanth@infinityinternationals.us";
            if (ToAddress == "")
            {
                ToAddress = "Soft-Team@infinityinternationals.us";
            }

            string Attachment = System.Web.HttpContext.Current.Server.MapPath(DtInvoice.Rows[0]["InvoiceAttachmentsPDF"].ToString());

            Hashtable htParam = new Hashtable();
            htParam.Add("EmailType", "");
            htParam.Add("To", Convert.ToString(ToAddress));
            htParam.Add("CC", ToCC);
            htParam.Add("BCC", ToBCC);
            htParam.Add("Subject", Subject);
            htParam.Add("Body", path);
            htParam.Add("Attachment", Attachment);
            htParam.Add("DraftNo", "1");
            int ReturnValue = InsertAutoEmailTracking(htParam);
        }

        public void SendTestEmailGroup(int InvoiceID, int AddedBy, int ClientID, string EmailIds)
        {
            StringBuilder htmlBody = new StringBuilder();
            StringBuilder htmlBody_Service = new StringBuilder();
            string ToAddress = "";
            string ToCC = "";
            string sub = "";
            string ToBCC = "";
            // string ToBCC = "s.swapnali@infinityinternationals.us";

            DataTable dt = new bllLogin().GetUserInformation(AddedBy);

            DataTable DTClientDetails = new bllTracking().GetClientDetails(ClientID);
            DataTable dtEmailDetailsByInvoiceID = new bllTracking().EmailDetailsByInvoiceIDGroup(InvoiceID);
            DataTable DtInvoice = blltracking.GetInvoiceDetailsGroup(InvoiceID);
            //string ClientEmail = Convert.ToString(DTClientDetails.Rows[0]["EmailID"]);
            if (DtInvoice != null)
            {
                if (DtInvoice.Rows.Count > 0)
                {

                    htmlBody_Service.Append("<table width=\"550px\" ><tr><td align=\"left\"><b>Dear Sir/Madam,</b></td></tr><tr>");
                    htmlBody_Service.Append("<td align=\"left\">Please find attached herewith following invoices.  Kindly confirm <b>"
                        + "</b> </td></tr></table><br /><table cellspacing='7px' cellpadding='3px' width='400px' style=' border-collapse: collapse;' border='1'\"><tr></tr></table>");
                    //htmlBody_Service.Append("<td align=\"left\">Invoice Number : </td><td align=\"bottom\">" + Convert.ToString(DtInvoice.Rows[0]["InvoiceNumber"]) + "</td></tr><tr>");
                    //htmlBody_Service.Append("<td align=\"left\">Project Number : </td><td align=\"bottom\">" + Convert.ToString(DtInvoice.Rows[0]["ProjectNumber"]) + "</td></tr><tr>");
                    //htmlBody_Service.Append("<td align=\"left\">Billing Period : </td><td align=\"bottom\">" + Convert.ToString(DtInvoice.Rows[0]["BillingPeriod"]) + "</td></tr></table>");

                    htmlBody_Service.Append("<br /><br /><table width=\"650px\"><tr><td align=\"left\">Thanks,<br />Infinity HRMS</td></tr> </table>");
                    //sendMailInvoice("New Invoice Created", "New Invoice  " + Convert.ToString(htProfile["Invoiceno"]) + " has been created.", htmlBody, Convert.ToString(dt.Rows[0]["OfficialEmailID"]));
                    string path = "E:/EmailPages/HRMS_BillingInvoiceGenerationtestEmail_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".html";
                    FileStream fp = File.Create(path);
                    fp.Close();
                    using (StreamWriter w = new StreamWriter(path, true))
                    {
                        w.WriteLine(Header + htmlBody_Service + Footer); // Write the text
                    }
                    string Subject = "Test Invoice Email for Billing " + Convert.ToString(DtInvoice.Rows[0]["InvoiceNumber"]);// +" - " + Convert.ToString(htProfile["Location"]);
                    // ToAddress = EmailIds; //Convert.ToString(dt.Rows[0]["OfficialEmailID"]);
                    ToAddress = "anita@infinity-data.com.pointofmail.com";
                    ToCC = "";
                    ToBCC = "n.nilkanth@infinityinternationals.us";
                    if (ToAddress == "")
                    {
                        ToAddress = "Soft-Team@infinityinternationals.us";
                    }

                    string Attachment = System.Web.HttpContext.Current.Server.MapPath(DtInvoice.Rows[0]["InvoiceAttachmentsPDF"].ToString());

                    Hashtable htParam = new Hashtable();
                    htParam.Add("EmailType", "");
                    htParam.Add("To", Convert.ToString(ToAddress));
                    htParam.Add("CC", ToCC);
                    htParam.Add("BCC", ToBCC);
                    htParam.Add("Subject", Subject);
                    htParam.Add("Body", path);
                    htParam.Add("Attachment", Attachment);
                    htParam.Add("DraftNo", "1");
                    int ReturnValue = InsertAutoEmailTracking(htParam);

                }
            }
        }

        public string ViewInvoiceEmailTemplate(int InvoiceID, int AddedBy, int ClientID, string Filepath)
        {
            StringBuilder htmlBody = new StringBuilder();
            StringBuilder htmlBody_Service = new StringBuilder();
            string ToAddress = "";
            string ToCC = "";
            string sub = "";
            string ToBCC = "";

            DataTable dt = new bllLogin().GetUserInformation(AddedBy);
            DataTable dtEmailDetailsByInvoiceID = new bllTracking().EmailDetailsByInvoiceID(InvoiceID);
            DataTable DTClientDetails = new bllTracking().GetClientDetails(ClientID);

            DataTable DtInvoice = blltracking.GetInvoiceDetails(InvoiceID);
            // string ClientEmail = Convert.ToString(DTClientDetails.Rows[0]["EmailID"]);


            htmlBody_Service.Append("<table width=\"550px\" ><tr><td align=\"left\"><b>Dear Sir/Madam,</b></td></tr><tr>");
            htmlBody_Service.Append("<td align=\"left\">Please find attached herewith following invoices.  Kindly confirm <b>"
                + "</b> </td></tr></table><br /><table cellspacing='7px' cellpadding='3px' width='400px' style=' border-collapse: collapse;' border='1'\"><tr></tr></table>");
            //htmlBody_Service.Append("<td align=\"left\">Invoice Number : </td><td align=\"bottom\">" + Convert.ToString(DtInvoice.Rows[0]["InvoiceNumber"]) + "</td></tr><tr>");
            //htmlBody_Service.Append("<td align=\"left\">Project Number : </td><td align=\"bottom\">" + Convert.ToString(DtInvoice.Rows[0]["ProjectNumber"]) + "</td></tr><tr>");
            //htmlBody_Service.Append("<td align=\"left\">Billing Period : </td><td align=\"bottom\">" + Convert.ToString(DtInvoice.Rows[0]["BillingPeriod"]) + "</td></tr></table>");

            htmlBody_Service.Append("<br /><br /><table width=\"650px\"><tr><td align=\"left\">Thanks,<br />Infinity HRMS</td></tr> </table>");
            //sendMailInvoice("New Invoice Created", "New Invoice  " + Convert.ToString(htProfile["Invoiceno"]) + " has been created.", htmlBody, Convert.ToString(dt.Rows[0]["OfficialEmailID"]));
            string path = "HRMS_TemplateEmail_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".html";
            string Filename = Filepath + path;
            FileStream fp = File.Create(Filename);
            fp.Close();

            using (StreamWriter w = new StreamWriter(Filename, true))
            {
                w.WriteLine(Header + htmlBody_Service + Footer); // Write the text
            }
            string Subject = "New Invoice for Billing " + Convert.ToString(DtInvoice.Rows[0]["InvoiceNumber"]);// +" - " + Convert.ToString(htProfile["Location"]);

            ToAddress = "anita@infinity-data.com.pointofmail.com";
            ToCC = "";
            ToBCC = "n.nilkanth@infinityinternationals.us";
            if (ToAddress == "")
            {
                ToAddress = "Soft-Team@infinityinternationals.us";
            }

            string Attachment = System.Web.HttpContext.Current.Server.MapPath(DtInvoice.Rows[0]["InvoiceAttachmentsPDF"].ToString());

            //Hashtable htParam = new Hashtable();
            //htParam.Add("EmailType", "Billing Invoice Template");
            //htParam.Add("To", Convert.ToString(ToAddress));
            //htParam.Add("CC", ToCC);
            //htParam.Add("BCC", ToBCC);
            //htParam.Add("Subject", Subject);
            //htParam.Add("Body", path);
            //htParam.Add("Attachment", Attachment);
            //htParam.Add("DraftNo", "1");
            //int ReturnValue = InsertAutoEmailTracking(htParam);
            return path;
        }

        public string ViewInvoiceEmailTemplateGroup(int InvoiceID, int AddedBy, int ClientID, string Filepath)
        {
            StringBuilder htmlBody = new StringBuilder();
            StringBuilder htmlBody_Service = new StringBuilder();
            string ToAddress = "";
            string ToCC = "";
            string sub = "";
            string ToBCC = "";

            DataTable dt = new bllLogin().GetUserInformation(AddedBy);
            DataTable dtEmailDetailsByInvoiceID = new bllTracking().EmailDetailsByInvoiceIDGroup(InvoiceID);
            DataTable DTClientDetails = new bllTracking().GetClientDetails(ClientID);

            DataTable DtInvoice = blltracking.GetInvoiceDetailsGroup(InvoiceID);
            // string ClientEmail = Convert.ToString(DTClientDetails.Rows[0]["EmailID"]);


            htmlBody_Service.Append("<table width=\"550px\" ><tr><td align=\"left\"><b>Dear Sir/Madam,</b></td></tr><tr>");
            htmlBody_Service.Append("<td align=\"left\">Please find attached herewith following invoices.  Kindly confirm <b>"
                + "</b> </td></tr></table><br /><table cellspacing='7px' cellpadding='3px' width='400px' style=' border-collapse: collapse;' border='1'\"><tr></tr></table>");
            //htmlBody_Service.Append("<td align=\"left\">Invoice Number : </td><td align=\"bottom\">" + Convert.ToString(DtInvoice.Rows[0]["InvoiceNumber"]) + "</td></tr><tr>");
            //htmlBody_Service.Append("<td align=\"left\">Project Number : </td><td align=\"bottom\">" + Convert.ToString(DtInvoice.Rows[0]["ProjectNumber"]) + "</td></tr><tr>");
            //htmlBody_Service.Append("<td align=\"left\">Billing Period : </td><td align=\"bottom\">" + Convert.ToString(DtInvoice.Rows[0]["BillingPeriod"]) + "</td></tr></table>");

            htmlBody_Service.Append("<br /><br /><table width=\"650px\"><tr><td align=\"left\">Thanks,<br />Infinity HRMS</td></tr> </table>");
            //sendMailInvoice("New Invoice Created", "New Invoice  " + Convert.ToString(htProfile["Invoiceno"]) + " has been created.", htmlBody, Convert.ToString(dt.Rows[0]["OfficialEmailID"]));
            string path = "HRMS_TemplateEmail_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".html";
            string Filename = Filepath + path;
            FileStream fp = File.Create(Filename);
            fp.Close();

            using (StreamWriter w = new StreamWriter(Filename, true))
            {
                w.WriteLine(Header + htmlBody_Service + Footer); // Write the text
            }
            string Subject = "New Invoice for Billing " + Convert.ToString(DtInvoice.Rows[0]["InvoiceNumber"]);// +" - " + Convert.ToString(htProfile["Location"]);

            ToAddress = "anita@infinity-data.com.pointofmail.com";
            ToCC = "";
            ToBCC = "n.nilkanth@infinityinternationals.us";
            if (ToAddress == "")
            {
                ToAddress = "Soft-Team@infinityinternationals.us";
            }

            string Attachment = System.Web.HttpContext.Current.Server.MapPath(DtInvoice.Rows[0]["InvoiceAttachmentsPDF"].ToString());

            //Hashtable htParam = new Hashtable();
            //htParam.Add("EmailType", "Billing Invoice Template");
            //htParam.Add("To", Convert.ToString(ToAddress));
            //htParam.Add("CC", ToCC);
            //htParam.Add("BCC", ToBCC);
            //htParam.Add("Subject", Subject);
            //htParam.Add("Body", path);
            //htParam.Add("Attachment", Attachment);
            //htParam.Add("DraftNo", "1");
            //int ReturnValue = InsertAutoEmailTracking(htParam);
            return path;
        }

        public int InsertAutoEmailTracking(Hashtable htParam)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_InsertAutoEmailTracking");
            SQLHelper.AddParamToSQLCmd(cmd, "@EmailType", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, htParam["EmailType"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@To", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, htParam["To"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@CC", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, htParam["CC"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@BCC", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, htParam["BCC"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@Subject", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, htParam["Subject"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@Body", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, htParam["Body"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@Attachment", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, htParam["Attachment"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@DraftNo", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, htParam["DraftNo"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);

            SQLHelper.ExecuteNonQueryCmdBilling(cmd);

            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            cmd.Dispose();
            return ReturnValue; //-1=Exist, 0=Fail, >0=Success
        }

        public void SendPriceEmail(int ProjectID, int DomainId)
        {
            StringBuilder htmlBody = new StringBuilder();
            StringBuilder htmlBody_Service = new StringBuilder();
            string ToAddress = "";
            string ToCC = "";
            string sub = "";
            string ToBCC = "";
            // string ToBCC = "s.swapnali@infinityinternationals.us";

            DataTable dt = new DALTracking().GetPriceDetailsByProjectId(ProjectID, DomainId);

            htmlBody_Service.Append("<table width=\"100%\" style='font-size:12px;'><tr><td align=\"left\"><b>Dear Sir/Madam,</b></td></tr><tr>");
            htmlBody_Service.Append("<td align=\"left\">Price for Project " + Convert.ToString(dt.Rows[0]["ProjectName"]) + " has been added in Billing software, please verify from your end.</td></tr></table><br />");
            //htmlBody_Service.Append("<table cellspacing='7px' cellpadding='3px' width='100%' style=' border-collapse:collapse; font-size:12px;' border='1'\"><tr><th>Sr. #</th><th>Parameter Name</th><th>Is Applicable?</th><th>Additional Charge</th><th>Charge Type</th><th>Price By BDM</th><th>Latest Price</th></tr>");
            htmlBody_Service.Append("<table cellspacing='7px' cellpadding='3px' width='100%' style=' border-collapse:collapse; font-size:12px;' border='1'\"><tr><th>Sr. #</th><th>Parameter Name</th><th>Is Applicable?</th><th>Additional Charge</th><th>Charge Type</th></tr>");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //htmlBody_Service.Append("<tr><td align='center'>" + Convert.ToString(i + 1) + "</td><td>" + Convert.ToString(dt.Rows[i]["IBP_ParameterName"]) + "</td><td align='center'>" + Convert.ToString(dt.Rows[i]["IBV_Comment"]) + "</td><td align='center'>" + Convert.ToString(dt.Rows[i]["IBV_Additional"]).Replace("Select", "No") + "</td><td align='center'>" + Convert.ToString(dt.Rows[i]["IBV_ChargeType"]).Replace("Select", "") + "</td><td align='center'>" + Convert.ToString(dt.Rows[i]["PriceFromBDM"]) + "</td><td align='center'>" + Convert.ToString(dt.Rows[i]["IBV_Remark"]) + "</td></tr>");
                    htmlBody_Service.Append("<tr><td align='center'>" + Convert.ToString(i + 1) + "</td><td>" + Convert.ToString(dt.Rows[i]["IBP_ParameterName"]) + "</td><td align='center'>" + Convert.ToString(dt.Rows[i]["IBV_Comment"]) + "</td><td align='center'>" + Convert.ToString(dt.Rows[i]["IBV_Additional"]).Replace("Select", "No") + "</td><td align='center'>" + Convert.ToString(dt.Rows[i]["IBV_ChargeType"]).Replace("Select", "") + "</td></tr>");
                }
            }
            htmlBody_Service.Append("</table><br /><br /><table width=\"100%\" style='font-size:12px;'><tr><td align=\"left\">Thanks,<br />Billing Software</td></tr> </table>");
            string path = "E:/EmailPages/ProjectPrice_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".html";
            FileStream fp = File.Create(path);
            fp.Close();
            using (StreamWriter w = new StreamWriter(path, true))
            {
                w.WriteLine(Header + htmlBody_Service + Footer); // Write the text
            }
            string Subject = "Price has been added for Project " + Convert.ToString(dt.Rows[0]["ProjectName"]);// +" - " + Convert.ToString(htProfile["Location"]);
            ToAddress = "anita@infinity-data.com.pointofmail.com";
            ToCC = "";
            ToBCC = "n.nilkanth@infinityinternationals.us";
            if (ToAddress == "")
            {
                ToAddress = "n.nilkanth@infinityinternationals.us";
            }
            StringBuilder MailBody = new StringBuilder();
            MailBody.Append(Header);
            MailBody.Append(htmlBody_Service);
            MailBody.Append(Footer);
            Hashtable htParam = new Hashtable();
            htParam.Add("EmailType", "");
            htParam.Add("To", Convert.ToString(ToAddress));
            htParam.Add("CC", ToCC);
            htParam.Add("BCC", ToBCC);
            htParam.Add("Subject", Subject);
            htParam.Add("Body", path);
            htParam.Add("Attachment", "");
            htParam.Add("DraftNo", "1");
            int ReturnValue = InsertAutoEmailTracking(htParam);
        }

        public void SendProjectCreationEmail(int ProjectID)
        {
            StringBuilder htmlBody = new StringBuilder();
            StringBuilder htmlBody_Service = new StringBuilder();
            string ToAddress = "";
            string ToCC = "";
            string ToBCC = "";

            DataTable dt = new DALTracking().GetProjectDetailsToSendToProductionTeam(ProjectID);

            htmlBody_Service.Append("<table width=\"100%\" style='font-size:12px;'><tr><td align=\"left\"><b>Dear Sir/Madam,</b></td></tr><tr>");
            htmlBody_Service.Append("<td align=\"left\">New Project has been added, please find details below.</td></tr></table><br />");
            //htmlBody_Service.Append("<table cellspacing='7px' cellpadding='3px' width='100%' style=' border-collapse:collapse; font-size:12px;' border='1'\"><tr><th>Sr. #</th><th>Parameter Name</th><th>Is Applicable?</th><th>Additional Charge</th><th>Charge Type</th><th>Price By BDM</th><th>Latest Price</th></tr>");
            htmlBody_Service.Append("<table cellspacing='7px' cellpadding='3px' width='100%' style=' border-collapse:collapse; font-size:12px;' border='1'\"><tr><th>Domain Name</th><th>Project Name</th><th>Process</th></tr>");
            if (dt.Rows.Count > 0)
            {
                htmlBody_Service.Append("<tr><td align='center'>" + Convert.ToString(dt.Rows[0]["DomainName"]) + "</td><td align='center'>" + Convert.ToString(dt.Rows[0]["ProjectName"]) + "</td><td align='center'>" + Convert.ToString(dt.Rows[0]["Process"]) + "</td></tr>");
            }
            htmlBody_Service.Append("</table><br /><br /><table width=\"100%\" style='font-size:12px;'><tr><td align=\"left\">Thanks,<br />Billing Software</td></tr> </table>");
            string path = "E:/EmailPages/ProjectDetails_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".html";
            FileStream fp = File.Create(path);
            fp.Close();
            using (StreamWriter w = new StreamWriter(path, true))
            {
                w.WriteLine(Header + htmlBody_Service + Footer); // Write the text
            }
            string Subject = "New project has been added";// +" - " + Convert.ToString(htProfile["Location"]);
            ToAddress = "anita@infinity-data.com.pointofmail.com";
            ToCC = "";
            ToBCC = "n.nilkanth@infinityinternationals.us";
            if (ToAddress == "")
            {
                ToAddress = "n.nilkanth@infinityinternationals.us";
            }
            StringBuilder MailBody = new StringBuilder();
            MailBody.Append(Header);
            MailBody.Append(htmlBody_Service);
            MailBody.Append(Footer);
            Hashtable htParam = new Hashtable();
            htParam.Add("EmailType", "");
            htParam.Add("To", Convert.ToString(ToAddress));
            htParam.Add("CC", ToCC);
            htParam.Add("BCC", ToBCC);
            htParam.Add("Subject", Subject);
            htParam.Add("Body", path);
            htParam.Add("Attachment", "");
            htParam.Add("DraftNo", "1");
            int ReturnValue = InsertAutoEmailTracking(htParam);
        }
    }
}
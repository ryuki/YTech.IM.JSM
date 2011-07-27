<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
 <input id="hidpaymentCash" name="hidpaymentCash" type="hidden" />
                    <input id="hidpaymentVoucher" name="hidpaymentVoucher" type="hidden" />
                    <input id="hidpaymentCreditCard" name="hidpaymentCreditCard" type="hidden" /> 
<div id="payment"> 
    <table>
        <tr>
            <td>
                <label for="paymentSubTotal">
                    Sub Total :</label>
            </td>
            <td align="right">
                <label id="paymentSubTotal">
                </label>
                <input id="hidPaymentSubTotal" name="hidPaymentSubTotal" type="hidden" />
            </td>
        </tr>
        <tr>
            <td>
                <label for="paymentDiscount">
                    Diskon :</label>
            </td>
            <td align="right">
                <label id="paymentDiscount">
                </label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <hr />
            </td>
        </tr>
        <tr>
            <td>
                <label for="paymentGrandTotal">
                    Grand Total :</label>
            </td>
            <td align="right">
                <label id="paymentGrandTotal">
                </label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
            </td>
        </tr>
        <tr>
            <td>
                <label for="paymentVoucher">
                    Voucher :</label>
            </td>
            <td align="right">
                <input id="paymentVoucher" name="paymentVoucher" type="text" />
            </td>
        </tr>
        <tr>
            <td>
                <label for="paymentCash">
                    Tunai :</label>
            </td>
            <td align="right">
                <input id="paymentCash" name="paymentCash" type="text" />
            </td>
        </tr>
        <tr>
            <td>
                <label for="paymentCreditCard">
                    Kartu Kredit :</label>
            </td>
            <td align="right">
                <input id="paymentCreditCard" name="paymentCreditCard" type="text" />
            </td>
        </tr>
        <tr>
            <td>
                <label for="paymentSisa">
                    Kembali :</label>
            </td>
            <td align="right">
                <label id="paymentSisa">
                </label>
            </td>
        </tr>
    </table>
</div>

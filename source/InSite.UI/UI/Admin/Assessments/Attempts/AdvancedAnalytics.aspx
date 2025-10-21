<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="AdvancedAnalytics.aspx.cs" 
    Inherits="InSite.UI.Admin.Assessments.AdvancedAnalytics"
    MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <style type="text/css">
        .input-number { width: 80px; }
    </style>

        <insite:Accordion runat="server">
            <insite:AccordionPanel runat="server" Title="Instructions" IsSelected="true" Icon="fas fa-chalkboard-teacher">

                <div class="row">
                    <div class="col-lg-6">

                        <div class="mb-3">
                            <insite:ComboBox runat="server">
                                <Items>
                                    <insite:ComboBoxOption Text="Assessment Form ABC-123" />
                                </Items>
                            </insite:ComboBox>
                        </div>

                        <p>
                        Check or enter ratings in the "First Round" tab, and "Second Round" tab if applicable.

                        If you have item statistics, enter them in the "Item Stats" panel. You can enter various values here to evaluate possible scenarios and provide your own estimated mean and SD.  

                        Enter Beuk and/or Hofstee ratings on the "Adjustments" tab.

                        Results will populate on the "Output" tab.
                        </p>

                    </div>
                </div>

            </insite:AccordionPanel>
            <insite:AccordionPanel runat="server" Title="First Round" Icon="fas fa-circle-notch">

                <table class="table table-striped table-bordered">
<tr>
    <th>Item</th>
    <th>Name</th>
    <th>Rater 1</th>
    <th>Rater 2</th>
    <th>Rater 3</th>
    <th>Rater 4</th>
    <th>Rater 5</th>
</tr>
<tr>
  <td>1</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="67"></td>
  <td><input class="form-control input-number" type="number" value="19"></td>
  <td><input class="form-control input-number" type="number" value="65"></td>
  <td><input class="form-control input-number" type="number" value="17"></td>
  <td><input class="form-control input-number" type="number" value="45"></td>
</tr>
<tr>
  <td>2</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="26"></td>
  <td><input class="form-control input-number" type="number" value="80"></td>
  <td><input class="form-control input-number" type="number" value="95"></td>
  <td><input class="form-control input-number" type="number" value="16"></td>
  <td><input class="form-control input-number" type="number" value="25"></td>
</tr>
<tr>
  <td>3</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="91"></td>
  <td><input class="form-control input-number" type="number" value="43"></td>
  <td><input class="form-control input-number" type="number" value="66"></td>
  <td><input class="form-control input-number" type="number" value="81"></td>
  <td><input class="form-control input-number" type="number" value="91"></td>
</tr>
<tr>
  <td>4</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="24"></td>
  <td><input class="form-control input-number" type="number" value="52"></td>
  <td><input class="form-control input-number" type="number" value="10"></td>
  <td><input class="form-control input-number" type="number" value="89"></td>
  <td><input class="form-control input-number" type="number" value="19"></td>
</tr>
<tr>
  <td>5</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="40"></td>
  <td><input class="form-control input-number" type="number" value="30"></td>
  <td><input class="form-control input-number" type="number" value="11"></td>
  <td><input class="form-control input-number" type="number" value="44"></td>
  <td><input class="form-control input-number" type="number" value="29"></td>
</tr>
<tr>
  <td>6</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="58"></td>
  <td><input class="form-control input-number" type="number" value="11"></td>
  <td><input class="form-control input-number" type="number" value="62"></td>
  <td><input class="form-control input-number" type="number" value="45"></td>
  <td><input class="form-control input-number" type="number" value="54"></td>
</tr>
<tr>
  <td>7</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="21"></td>
  <td><input class="form-control input-number" type="number" value="25"></td>
  <td><input class="form-control input-number" type="number" value="89"></td>
  <td><input class="form-control input-number" type="number" value="84"></td>
  <td><input class="form-control input-number" type="number" value="9"></td>
</tr>
<tr>
  <td>8</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="63"></td>
  <td><input class="form-control input-number" type="number" value="15"></td>
  <td><input class="form-control input-number" type="number" value="41"></td>
  <td><input class="form-control input-number" type="number" value="80"></td>
  <td><input class="form-control input-number" type="number" value="55"></td>
</tr>
<tr>
  <td>9</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="87"></td>
  <td><input class="form-control input-number" type="number" value="83"></td>
  <td><input class="form-control input-number" type="number" value="83"></td>
  <td><input class="form-control input-number" type="number" value="29"></td>
  <td><input class="form-control input-number" type="number" value="6"></td>
</tr>
<tr>
  <td>10</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="60"></td>
  <td><input class="form-control input-number" type="number" value="4"></td>
  <td><input class="form-control input-number" type="number" value="10"></td>
  <td><input class="form-control input-number" type="number" value="50"></td>
  <td><input class="form-control input-number" type="number" value="93"></td>
</tr>
<tr>
  <td>11</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="17"></td>
  <td><input class="form-control input-number" type="number" value="30"></td>
  <td><input class="form-control input-number" type="number" value="97"></td>
  <td><input class="form-control input-number" type="number" value="46"></td>
  <td><input class="form-control input-number" type="number" value="18"></td>
</tr>
<tr>
  <td>12</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="38"></td>
  <td><input class="form-control input-number" type="number" value="8"></td>
  <td><input class="form-control input-number" type="number" value="21"></td>
  <td><input class="form-control input-number" type="number" value="88"></td>
  <td><input class="form-control input-number" type="number" value="87"></td>
</tr>
<tr>
  <td>13</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="16"></td>
  <td><input class="form-control input-number" type="number" value="19"></td>
  <td><input class="form-control input-number" type="number" value="50"></td>
  <td><input class="form-control input-number" type="number" value="61"></td>
  <td><input class="form-control input-number" type="number" value="4"></td>
</tr>
<tr>
  <td>14</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="67"></td>
  <td><input class="form-control input-number" type="number" value="54"></td>
  <td><input class="form-control input-number" type="number" value="81"></td>
  <td><input class="form-control input-number" type="number" value="73"></td>
  <td><input class="form-control input-number" type="number" value="29"></td>
</tr>
<tr>
  <td>15</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="10"></td>
  <td><input class="form-control input-number" type="number" value="11"></td>
  <td><input class="form-control input-number" type="number" value="29"></td>
  <td><input class="form-control input-number" type="number" value="35"></td>
  <td><input class="form-control input-number" type="number" value="31"></td>
</tr>
<tr>
  <td>16</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="56"></td>
  <td><input class="form-control input-number" type="number" value="46"></td>
  <td><input class="form-control input-number" type="number" value="81"></td>
  <td><input class="form-control input-number" type="number" value="39"></td>
  <td><input class="form-control input-number" type="number" value="68"></td>
</tr>
<tr>
  <td>17</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="54"></td>
  <td><input class="form-control input-number" type="number" value="1"></td>
  <td><input class="form-control input-number" type="number" value="13"></td>
  <td><input class="form-control input-number" type="number" value="51"></td>
  <td><input class="form-control input-number" type="number" value="38"></td>
</tr>
<tr>
  <td>18</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="91"></td>
  <td><input class="form-control input-number" type="number" value="28"></td>
  <td><input class="form-control input-number" type="number" value="44"></td>
  <td><input class="form-control input-number" type="number" value="67"></td>
  <td><input class="form-control input-number" type="number" value="70"></td>
</tr>
<tr>
  <td>19</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="55"></td>
  <td><input class="form-control input-number" type="number" value="94"></td>
  <td><input class="form-control input-number" type="number" value="53"></td>
  <td><input class="form-control input-number" type="number" value="93"></td>
  <td><input class="form-control input-number" type="number" value="39"></td>
</tr>
<tr>
  <td>20</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="69"></td>
  <td><input class="form-control input-number" type="number" value="33"></td>
  <td><input class="form-control input-number" type="number" value="23"></td>
  <td><input class="form-control input-number" type="number" value="25"></td>
  <td><input class="form-control input-number" type="number" value="39"></td>
</tr>
<tr>
    <th></th>
    <th>Rater Average (%)</th>
    <th>69.5</th>
    <th>67.9</th>
    <th>66.8</th>
    <th>71.2</th>
    <th>67.6</th>
</tr>
<tr>
    <th></th>
    <th>Rater Sum (Raw)</th>
    <th>139.0</th>
    <th>135.7</th>
    <th>133.6</th>
    <th>142.3</th>
    <th>135.2</th>
</tr>
</table>

            </insite:AccordionPanel>
            <insite:AccordionPanel runat="server" Title="Second Round" Icon="fas fa-circle">

                
<table class="table table-striped table-bordered">
<tr>
    <th>Item</th>
    <th>Name</th>
    <th>Rater 1</th>
    <th>Rater 2</th>
    <th>Rater 3</th>
    <th>Rater 4</th>
    <th>Rater 5</th>
</tr>
<tr>
  <td>1</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="78"></td>
  <td><input class="form-control input-number" type="number" value="32"></td>
  <td><input class="form-control input-number" type="number" value="35"></td>
  <td><input class="form-control input-number" type="number" value="76"></td>
  <td><input class="form-control input-number" type="number" value="55"></td>
</tr>
<tr>
  <td>2</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="9"></td>
  <td><input class="form-control input-number" type="number" value="25"></td>
  <td><input class="form-control input-number" type="number" value="19"></td>
  <td><input class="form-control input-number" type="number" value="34"></td>
  <td><input class="form-control input-number" type="number" value="21"></td>
</tr>
<tr>
  <td>3</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="64"></td>
  <td><input class="form-control input-number" type="number" value="82"></td>
  <td><input class="form-control input-number" type="number" value="7"></td>
  <td><input class="form-control input-number" type="number" value="67"></td>
  <td><input class="form-control input-number" type="number" value="15"></td>
</tr>
<tr>
  <td>4</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="50"></td>
  <td><input class="form-control input-number" type="number" value="97"></td>
  <td><input class="form-control input-number" type="number" value="17"></td>
  <td><input class="form-control input-number" type="number" value="94"></td>
  <td><input class="form-control input-number" type="number" value="77"></td>
</tr>
<tr>
  <td>5</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="17"></td>
  <td><input class="form-control input-number" type="number" value="18"></td>
  <td><input class="form-control input-number" type="number" value="79"></td>
  <td><input class="form-control input-number" type="number" value="10"></td>
  <td><input class="form-control input-number" type="number" value="83"></td>
</tr>
<tr>
  <td>6</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="37"></td>
  <td><input class="form-control input-number" type="number" value="50"></td>
  <td><input class="form-control input-number" type="number" value="94"></td>
  <td><input class="form-control input-number" type="number" value="7"></td>
  <td><input class="form-control input-number" type="number" value="25"></td>
</tr>
<tr>
  <td>7</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="14"></td>
  <td><input class="form-control input-number" type="number" value="93"></td>
  <td><input class="form-control input-number" type="number" value="84"></td>
  <td><input class="form-control input-number" type="number" value="98"></td>
  <td><input class="form-control input-number" type="number" value="75"></td>
</tr>
<tr>
  <td>8</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="14"></td>
  <td><input class="form-control input-number" type="number" value="38"></td>
  <td><input class="form-control input-number" type="number" value="71"></td>
  <td><input class="form-control input-number" type="number" value="92"></td>
  <td><input class="form-control input-number" type="number" value="84"></td>
</tr>
<tr>
  <td>9</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="97"></td>
  <td><input class="form-control input-number" type="number" value="89"></td>
  <td><input class="form-control input-number" type="number" value="6"></td>
  <td><input class="form-control input-number" type="number" value="76"></td>
  <td><input class="form-control input-number" type="number" value="88"></td>
</tr>
<tr>
  <td>10</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="89"></td>
  <td><input class="form-control input-number" type="number" value="59"></td>
  <td><input class="form-control input-number" type="number" value="67"></td>
  <td><input class="form-control input-number" type="number" value="77"></td>
  <td><input class="form-control input-number" type="number" value="94"></td>
</tr>
<tr>
  <td>11</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="88"></td>
  <td><input class="form-control input-number" type="number" value="58"></td>
  <td><input class="form-control input-number" type="number" value="24"></td>
  <td><input class="form-control input-number" type="number" value="31"></td>
  <td><input class="form-control input-number" type="number" value="22"></td>
</tr>
<tr>
  <td>12</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="60"></td>
  <td><input class="form-control input-number" type="number" value="53"></td>
  <td><input class="form-control input-number" type="number" value="25"></td>
  <td><input class="form-control input-number" type="number" value="93"></td>
  <td><input class="form-control input-number" type="number" value="19"></td>
</tr>
<tr>
  <td>13</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="58"></td>
  <td><input class="form-control input-number" type="number" value="30"></td>
  <td><input class="form-control input-number" type="number" value="13"></td>
  <td><input class="form-control input-number" type="number" value="10"></td>
  <td><input class="form-control input-number" type="number" value="7"></td>
</tr>
<tr>
  <td>14</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="70"></td>
  <td><input class="form-control input-number" type="number" value="98"></td>
  <td><input class="form-control input-number" type="number" value="8"></td>
  <td><input class="form-control input-number" type="number" value="91"></td>
  <td><input class="form-control input-number" type="number" value="2"></td>
</tr>
<tr>
  <td>15</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="13"></td>
  <td><input class="form-control input-number" type="number" value="27"></td>
  <td><input class="form-control input-number" type="number" value="25"></td>
  <td><input class="form-control input-number" type="number" value="10"></td>
  <td><input class="form-control input-number" type="number" value="80"></td>
</tr>
<tr>
  <td>16</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="28"></td>
  <td><input class="form-control input-number" type="number" value="13"></td>
  <td><input class="form-control input-number" type="number" value="4"></td>
  <td><input class="form-control input-number" type="number" value="22"></td>
  <td><input class="form-control input-number" type="number" value="93"></td>
</tr>
<tr>
  <td>17</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="77"></td>
  <td><input class="form-control input-number" type="number" value="83"></td>
  <td><input class="form-control input-number" type="number" value="18"></td>
  <td><input class="form-control input-number" type="number" value="12"></td>
  <td><input class="form-control input-number" type="number" value="37"></td>
</tr>
<tr>
  <td>18</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="56"></td>
  <td><input class="form-control input-number" type="number" value="69"></td>
  <td><input class="form-control input-number" type="number" value="54"></td>
  <td><input class="form-control input-number" type="number" value="77"></td>
  <td><input class="form-control input-number" type="number" value="16"></td>
</tr>
<tr>
  <td>19</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="61"></td>
  <td><input class="form-control input-number" type="number" value="13"></td>
  <td><input class="form-control input-number" type="number" value="78"></td>
  <td><input class="form-control input-number" type="number" value="74"></td>
  <td><input class="form-control input-number" type="number" value="27"></td>
</tr>
<tr>
  <td>20</td>
  <td>-</td>
  <td><input class="form-control input-number" type="number" value="67"></td>
  <td><input class="form-control input-number" type="number" value="77"></td>
  <td><input class="form-control input-number" type="number" value="95"></td>
  <td><input class="form-control input-number" type="number" value="70"></td>
  <td><input class="form-control input-number" type="number" value="18"></td>
</tr>
<tr>
    <th></th>
    <th>Rater Average (%)</th>
    <th>69.5</th>
    <th>67.9</th>
    <th>66.8</th>
    <th>71.2</th>
    <th>67.6</th>
</tr>
<tr>
    <th></th>
    <th>Rater Sum (Raw)</th>
    <th>139.0</th>
    <th>135.7</th>
    <th>133.6</th>
    <th>142.3</th>
    <th>135.2</th>
</tr>
</table>


            </insite:AccordionPanel>
            <insite:AccordionPanel runat="server" Title="Item Statistics" Icon="fas fa-square-root">

                <table class="table table-striped table-bordered">
                <tr>
                    <th>Sequence</th>
                    <th>ID</th>
                    <th class="text-end">P</th>
                    <th class="text-end">Rpbis</th>
                    <th class="text-end">PQ</th>
                    <th class="text-end">sqrt(PQ)</th>
                    <th class="text-end">ItemRel</th>
                </tr>
                <tr>
                  <td>1</td>
                  <td>Item1</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>2</td>
                  <td>Item2</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>3</td>
                  <td>Item3</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>4</td>
                  <td>Item4</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>5</td>
                  <td>Item5</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>6</td>
                  <td>Item6</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>7</td>
                  <td>Item7</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>8</td>
                  <td>Item8</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>9</td>
                  <td>Item9</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>10</td>
                  <td>Item10</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>11</td>
                  <td>Item11</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>12</td>
                  <td>Item12</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>13</td>
                  <td>Item13</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>14</td>
                  <td>Item14</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>15</td>
                  <td>Item15</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>16</td>
                  <td>Item16</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>17</td>
                  <td>Item17</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>18</td>
                  <td>Item18</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>19</td>
                  <td>Item19</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                <tr>
                  <td>20</td>
                  <td>Item20</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                  <td class="text-end">0.00</td>
                </tr>
                </table>

                <div class="row">
                    <div class="col-lg-5">

                        <h3 class="h5">Summary</h3>

                        <table class="table table-bordered table-striped">
                            <tr>
                                <td>Number of Items</td>
                                <td class="text-end">-</td>
                            </tr>
                            <tr>
                                <td>Projected Mean from Item Statistics</td>
                                <td class="text-end">-</td>
                            </tr>
                            <tr>
                                <td>Projected SD from Item Statistics</td>
                                <td class="text-end">-</td>
                            </tr>
                            <tr>
                                <td>Projected KR20 from Item Statistics</td>
                                <td class="text-end">-</td>
                            </tr>
                            <tr>
                                <td>Projected Mean from Other Source</td>
                                <td class="text-end">135.64</td>
                            </tr>
                            <tr>
                                <td>Projected SD from Other Source</td>
                                <td class="text-end">5.80</td>
                            </tr>
                        </table>

                    </div>
                </div>

            </insite:AccordionPanel>
            <insite:AccordionPanel runat="server" Title="Adjustments" Icon="fas fa-triangle">

                <div class="row">
                    <div class="col-lg-7">

<table class="table table-bordered table-striped">
<tr>
    <th>Rater</th>
    <th>Beuk</th>
    <th>Min Fail Rate</th>
    <th>Max Fail Rate</th>
    <th>Min Cut Score</th>
    <th>Max Cut Score</th>
</tr>
<tr>
  <td>1</td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
</tr>
<tr>
  <td>2</td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
</tr>
<tr>
  <td>3</td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
</tr>
<tr>
  <td>4</td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
</tr>
<tr>
  <td>5</td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
  <td><input class="form-control input-number" type="number" value=""></td>
</tr>
                        </table>

                    </div>
                </div>

            </insite:AccordionPanel>
            <insite:AccordionPanel runat="server" Title="Outputs" Icon="fas fa-flag-checkered">

                <div class="row">
                    <div class="col-lg-5">

                        <h3 class="h5">First Round</h3>

                        <table class="table table-bordered table-striped">
                            <tr>
                                <th>Statistic</th>
                                <th class="text-end">Raw Scale</th>
                                <th class="text-end">% Scale</th>
                            </tr>
                            <tr>
                                <td>Average</td>
                                <td class="text-end">136.98</td>
                                <td class="text-end">68.49</td>
                            </tr>
                            <tr>
                                <td>Standard Deviation</td>
                                <td class="text-end">3.13</td>
                                <td class="text-end">1.57</td>
                            </tr>
                            <tr>
                                <td>Minimum</td>
                                <td class="text-end">133.60</td>
                                <td class="text-end">66.80</td>
                            </tr>
                            <tr>
                                <td>Maximum</td>
                                <td class="text-end">142.30</td>
                                <td class="text-end">71.15</td>
                            </tr>
                            <tr>
                                <td>Sample SE</td>
                                <td class="text-end">1.28</td>
                                <td class="text-end">0.64</td>
                            </tr>
                            <tr>
                                <td>IRR-Based SE</td>
                                <td class="text-end">1.55</td>
                                <td class="text-end">0.78</td>
                            </tr>
                            <tr>
                                <td>Maximum Possible</td>
                                <td class="text-end">200</td>
                                <td class="text-end">100</td>
                            </tr>
                            <tr>
                                <td>Projected Raw Mean</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                            </tr>
                            <tr>
                                <td>Projected Raw SD</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                            </tr>
                            <tr>
                                <td>Inter-Rater Reliability</td>
                                <td class="text-end">0.755</td>
                                <td class="text-end">-</td>
                            </tr>
                            <tr>
                                <td>Number of Raters</td>
                                <td class="text-end">5</td>
                                <td class="text-end">-</td>
                            </tr>
                        </table>

                        <h3 class="h5">Cuts</h3>

                        <table class="table table-bordered table-striped">
                            <tr>
                                <th></th>
                                <th class="text-end">Cut - SEJ</th>
                                <th class="text-end">Cut</th>
                                <th class="text-end">Cut + SEJ</th>
                            </tr>
                            <tr>
                                <td>Panel Recommendation</td>
                                <td class="text-end">135.43</td>
                                <td class="text-end">136.98</td>
                                <td class="text-end">138.53</td>
                            </tr>
                            <tr>
                                <td class="ps-4">Pass Rate</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                            </tr>
                            <tr>
                                <td>Beuk</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                            </tr>
                            <tr>
                                <td class="ps-4">Pass Rate</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                            </tr>
                            <tr>
                                <td>Hofstee</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                            </tr>
                            <tr>
                                <td class="ps-4">Pass Rate</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                            </tr>
                        </table>

                    </div>
                    <div class="col-lg-5">

                        <h3 class="h5">Second Round</h3>

                        <table class="table table-bordered table-striped">
                            <tr>
                                <th>Statistic</th>
                                <th class="text-end">Raw Scale</th>
                                <th class="text-end">% Scale</th>
                            </tr>
                            <tr>
                                <td>Average</td>
                                <td class="text-end">136.63</td>
                                <td class="text-end">69.32</td>
                            </tr>
                            <tr>
                                <td>Standard Deviation</td>
                                <td class="text-end">2.98</td>
                                <td class="text-end">1.49</td>
                            </tr>
                            <tr>
                                <td>Minimum</td>
                                <td class="text-end">135.80</td>
                                <td class="text-end">67.90</td>
                            </tr>
                            <tr>
                                <td>Maximum</td>
                                <td class="text-end">142.80</td>
                                <td class="text-end">71.40</td>
                            </tr>
                            <tr>
                                <td>Sample SE</td>
                                <td class="text-end">1.21</td>
                                <td class="text-end">0.61</td>
                            </tr>
                            <tr>
                                <td>IRR-Based SE</td>
                                <td class="text-end">0.99</td>
                                <td class="text-end">0.49</td>
                            </tr>
                            <tr>
                                <td>Maximum Possible</td>
                                <td class="text-end">200</td>
                                <td class="text-end">100</td>
                            </tr>
                            <tr>
                                <td>Projected Raw Mean</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                            </tr>
                            <tr>
                                <td>Projected Raw SD</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                            </tr>
                            <tr>
                                <td>Inter-Rater Reliability</td>
                                <td class="text-end">0.889</td>
                                <td class="text-end">-</td>
                            </tr>
                            <tr>
                                <td>Number of Raters</td>
                                <td class="text-end">5</td>
                                <td class="text-end">-</td>
                            </tr>
                        </table>

                        <h3 class="h5">Cuts</h3>

                        <table class="table table-bordered table-striped">
                            <tr>
                                <th></th>
                                <th class="text-end">Cut - SEJ</th>
                                <th class="text-end">Cut</th>
                                <th class="text-end">Cut + SEJ</th>
                            </tr>
                            <tr>
                                <td>Panel Recommendation</td>
                                <td class="text-end">137.64</td>
                                <td class="text-end">138.63</td>
                                <td class="text-end">139.62</td>
                            </tr>
                            <tr>
                                <td class="ps-4">Pass Rate</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                            </tr>
                            <tr>
                                <td>Beuk</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                            </tr>
                            <tr>
                                <td class="ps-4">Pass Rate</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                            </tr>
                            <tr>
                                <td>Hofstee</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                            </tr>
                            <tr>
                                <td class="ps-4">Pass Rate</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                                <td class="text-end">-</td>
                            </tr>
                        </table>

                    </div>
                </div>

            </insite:AccordionPanel>
            <insite:AccordionPanel runat="server" Title="Charts" Icon="fas fa-chart-line">

                <div class="row">
                    <div class="col-lg-5">

                        <h3 class="h5">First Round Beuk</h3>
                        <img src="/UI/Admin/Assessments/Attempts/Content/First-Round-Beuk.png" />

                    </div>
                    <div class="col-lg-5">

                        <h3 class="h5">Second Round Beuk</h3>
                        <img src="/UI/Admin/Assessments/Attempts/Content/Second-Round-Beuk.png" />

                        <h3 class="h5 mt-4">Hofstee</h3>
                        <img src="/UI/Admin/Assessments/Attempts/Content/Hofstee.png" />

                    </div>
                </div>

            </insite:AccordionPanel>
        </insite:Accordion>

</asp:Content>
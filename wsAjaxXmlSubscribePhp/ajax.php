<?php 

namespace wsAjaxXmlSubscribe;

class ajax
{
	/**
	 * Summary of $consoleUrl
	 * @var mixed
     * Type in #consoleurl# your console url 
     * and make sure frontend web methods are enabled under your console configuration.
     * Read more at http://help.mailup.com/display/mailupUserGuide/Connecting+to+MailUp).
	 */
	protected $consoleUrl = 'http://#consoleurl#/frontend/xmlsubscribe.aspx'; 
 
	function Page_Load()
	{
        // check if ajax callback
        if( isset($_SERVER['HTTP_X_REQUESTED_WITH']) && $_SERVER['HTTP_X_REQUESTED_WITH'] == 'XMLHttpRequest' )
        {
            // check if request method is post
			if ($_SERVER['REQUEST_METHOD']=='POST')
			{
                //receive params
                $Language = $_POST['Lingua'] ? : '';
				$Email = $_POST['Email'] ? : '';
				$Nome = $_POST['Nome'] ? : '';
				$IDList = $_POST['Lista'] ? : '';
				$IDGroup = $_POST['Gruppo'] ? : '';
				$RequiredConfirmation = $_POST['RequireConfirmation'] ? : '';
				$csvFldNames = $_POST['csvFldNames'] ? : '';
				$csvFldValues = $_POST['csvFldValues'] ? : '';
				$ReturnCode = $_POST['ReturnCode'] ? : '';
				$Number = '';
                
                
                if (!is_null($_POST['Prefix']) and !is_null($_POST['Number']) )
                {
                    //phone number formatting
                   $Number = $_POST['Prefix'].''.str_replace($_POST['Number'],'+','00');
                   
                   //remote callback to MailUp frontend method
                   $response = $this->CallXmlSubscribeConsole($Email, $Nome,$IDList,$IDGroup,$Language,$RequiredConfirmation,$csvFldNames,$csvFldValues,$ReturnCode,$Number);
                   
                   //prepare for output
                   header('Content-Type: application/json');
                   
                   //output result
                   die('{"Message":'.json_encode($response)."}"); 
                } 
			}			
		}

	}
    
    function CallXmlSubscribeConsole($Email,$Nome, $IDList, $IDGroup, $Language, $RequiredConfirmation, $csvFldNames, $csvFldValues, $ReturnCode, $Number)
    {
        $server_response = '';
        try
        {
            //prepare query string
            $console_url = $this->consoleUrl.'?email='.urldecode($Email).'&name='.$Nome.'&list='.$IDList.'&group='.$IDGroup.'&lang='.$Language.'&confirm='.$RequiredConfirmation.'&csvFldNames='.$csvFldNames.'&csvFldValues='.$csvFldValues.'&retCode='.$ReturnCode.'&sms='.$Number;
            //init callback
            $server_request = curl_init($console_url);
            // set return value option
            curl_setopt($server_request, CURLOPT_RETURNTRANSFER, 1);
            //expect result
            $server_response = curl_exec($server_request);
            //close callback
            curl_close($server_request);  
            //return server response
            return $server_response;
        }
        catch (Exception $ex)
        {
            throw $ex;
        }
    }

}


$testObject = new ajax();

$testObject->Page_Load();




?>


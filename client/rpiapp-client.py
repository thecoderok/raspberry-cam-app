import time
import swagger_client
from swagger_client.rest import ApiException
from pprint import pprint
# create an instance of the API class
api_instance = swagger_client.PhotoEntryApi()

try:
    api_response = api_instance.api_photo_entry_get()
    pprint(api_response)
except ApiException as e:
    print "Exception when calling PhotoEntryApi->api_photo_entry_get: %s\n" % e
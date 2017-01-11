# swagger_client.PhotoEntryApi

All URIs are relative to *https://localhost/*

Method | HTTP request | Description
------------- | ------------- | -------------
[**api_photo_entry_get**](PhotoEntryApi.md#api_photo_entry_get) | **GET** /api/PhotoEntry | 


# **api_photo_entry_get**
> list[PhotoEntry] api_photo_entry_get()



### Example 
```python
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
```

### Parameters
This endpoint does not need any parameter.

### Return type

[**list[PhotoEntry]**](PhotoEntry.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


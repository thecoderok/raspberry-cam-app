import requests
import json

endpoint = "http://localhost:5000/"

def get_token(username, password):
    print "Getting authentication token"
    headers = {'Content-Type': 'application/x-www-form-urlencoded'}
    payload = {'username': username, 'password': password}
    response = requests.post(endpoint + 'token', headers=headers, data=payload)
    if response.status_code != 200:
        raise Exception('Failed to obtain token: ' + str(response.status_code))

    print "Request to /token succeeded"
    decoded = json.loads(response.content)
    
    # print json.dumps(decoded)
    token = decoded['access_token']
    if not token:
        raise Exception("No token present in response: " + response.content)
    
    return token

def get_entries(token):
    headers = { 'Accept': 'application/json', 'Authorization': 'Bearer ' + token }
    url = endpoint + "api/PhotoEntry"
    response = requests.get(url, headers=headers)
    if response.status_code != 200:
        raise Exception('Failed to obtain entries: ' + str(response.status_code))
    
    print "Obtained entries"
    decoded = json.loads(response.content)
    print json.dumps(decoded)

token = get_token("TEST", "TEST123")
get_entries(token)

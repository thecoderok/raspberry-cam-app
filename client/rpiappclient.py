import requests
import json
import ConfigParser, os
import sys

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

def post_photo(token, path_to_file):
    headers = {'Accept': 'application/json', 'Authorization': 'Bearer ' + token}
    url = endpoint + "api/PhotoEntry"
    f = open(path_to_file, 'rb')
    response = requests.post(url, headers=headers, files={'file': f})
    if response.status_code != 200:
        raise Exception('Failed to post photo: ' + str(response.status_code))
    print "Photo posted successfully"

def read_config():
    dir_path = os.path.dirname(os.path.realpath(__file__))
    config = ConfigParser.ConfigParser()
    config.readfp(open(dir_path + '/config.ini'))
    return config

if len(sys.argv) < 2:
    raise Exception("File to upload should be passed as a command line argument")

file_name = sys.argv[1]

config = read_config()
user=config.get('Auth', 'user')
password=config.get('Auth', 'password')
endpoint=config.get('Auth', 'endpoint')
token = get_token(user, password)
get_entries(token)
post_photo(token, file_name)
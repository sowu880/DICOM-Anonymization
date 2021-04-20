import requests
from bs4 import BeautifulSoup
# Url of website
url="http://dicom.nema.org/medical/dicom/2018e/output/chtml/part15/chapter_E.html"
rawdata=requests.get(url)
html=rawdata.content
soup = BeautifulSoup(html, 'html.parser')
table = soup.find_all('tr')
for ind,item in enumerate(table[4:309]):
    tag = item.contents[3].contents[1].contents[2]
    method = item.contents[9].contents[1].contents[2]
    name = item.contents[1].contents[1].contents[2]
    if 'X' in method:
        method = "remove"
    if method=='Z':
        method = "redact"
    if method=='U':
        method = "refreshUID"
    print("\t{\"tag\":\"%s\", \"method\":\"%s\"}, //%s" %(tag, method, name))

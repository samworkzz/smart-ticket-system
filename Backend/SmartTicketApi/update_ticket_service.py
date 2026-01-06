import re

file_path = r"c:\Users\hp\SmartTicketSystem\Backend\SmartTicketApi\Services\Tickets\TicketService.cs"

with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

# Pattern to find TicketListDto mappings and add IsEscalated
# We need to add it before the closing })
pattern1 = r'(AssignedTo = t\.AssignedTo != null \? t\.AssignedTo\.Name : null)\r?\n(\s+)\}\)'
replacement1 = r'\1,\r\n\2IsEscalated = t.IsEscalated\r\n\2})'

pattern2 = r'(AssignedTo = t\.AssignedTo!\.Name)\r?\n(\s+)\}\)'
replacement2 = r'\1,\r\n\2IsEscalated = t.IsEscalated\r\n\2})'

content = re.sub(pattern1, replacement1, content)
content = re.sub(pattern2, replacement2, content)

with open(file_path, 'w', encoding='utf-8') as f:
    f.write(content)

print("File updated successfully")

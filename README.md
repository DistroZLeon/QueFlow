# QueFlow
This is a C# using ASP.NET MVC project for the Web Application Design class.

“Collective Knowledge” platform with the following functionalities:

- There should be 3 types of users: unregistered visitor, registered user and
administrator.
- Unregistered users can see the platform presentation page and the
authentication and registration forms.
- The platform presentation page must contain the elements described in the following requirement. If
it does not contain other elements besides login and register, the
full score is not awarded.
- On the first page (presentation page) several
discussion categories will appear. These will appear in the form of tags. In each
category/tag there will be several discussion topics. These
discussion topics will actually be questions that users have.
- Any user (unregistered and registered) can view the
discussion topics (questions) from different categories and the answers to the respective
questions.
- Topics in a category can be viewed in a new page. The category is accessed in a new page, the page where all the discussion topics (questions) in that category will be displayed. If a category has multiple discussion topics, they will be displayed in pagination .
- Each discussion topic will have answers from other users.
Discussions and answers will be displayed sorted according to criteria established by the user (two criteria are chosen; depending on the date the discussion topic was posted, depending on the number of answers, alphabetically depending on the discussion topic, etc.).
- The platform will have its own search engine with which users can search for different discussion topics and answers. The search will be done by keywords found in the discussion topics and answers. The search does not necessarily have to be done using a word written in full. A user can only search for certain parts that make up the name. (e.g. SEPT can be searched for instead of SEPTEMBER).
- Registered users can create a profile that they can also edit. The profile must have the full name, e-mail address, description (about section). The profile will also show recent activity - the latest questions and answers posted by the user.
- Registered users can initiate new discussion topics and can send replies to other discussions . Unregistered users can only view. Discussion topics and messages already sent can be modified and later deleted by the author.
- The administrator is responsible for deleting discussions and messages that have inappropriate content and for placing discussion topics in their appropriate category. If a discussion topic is not in the appropriate category, the moderator can make this change.
- The administrator can perform CRUD on categories (can view, create,
modify and delete). The administrator is also the one who associates or
revoke members' rights. 

# Job Alert Application

- ##### Author:
  
  Hugo Perreault Gravel

- ##### Description:
This project is a job alert application developed in C#. It scrapes job postings from various websites and stores the results in JSON format. The application features an automated background service that performs daily scraping, ensuring users receive timely updates about new job postings. Utilizing the observer design pattern, it manages notifications, allowing users to receive alerts via email whenever new opportunities arise. Additionally, the application is configurable, enabling users to customize job sites, XPaths, and scraping behaviors through JSON configuration files. It can also be deployed using Docker, providing a flexible solution for running the application in different environments.

## Prerequisites

- .NET SDK

## Usage

### Build

```
make build
```

### Start the Application

```
make run
```

### Daily Scraping

```
make daily
```

### Publish the Application

```
make publish
```

### Clean Build

```
make clean
```

## Environment Variables

To configure the application, create a `.env` file in the root directory of your project. This file should contain the following variables:

```
SMTP_USERNAME=your_email@gmail.com
SMTP_PASSWORD=your_email_password
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
TEST_END_USER="recipient_email@gmail.com"
```

Make sure to replace the placeholders with your actual SMTP credentials and recipient email address. This information is essential for the email notification feature to work properly.

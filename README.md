# Electricity Bill Service
The Electricity Bill Service is a .NET-based microservice that allows users to verify and pay electricity bills, manage wallets, and receive SMS notifications for events such as bill creation, successful payments, and fund additions.

## Table of Contents
1. Setup and Run Instructions
2. Design Decisions
3. Event Handling and Messaging
4.SMS Notification Configuration
5. Setup and Run Instructions

## Prerequisites
1. Docker and Docker Compose installed on your machine.
2. LocalStack for emulating AWS services locally.
3. .NET 6 SDK for local development if needed.

## Running the Service
1. Clone the Repository:

```bash

git clone <repository-url>
cd electricity-bill-service
```
2. Setup Environment Variables:
 In the appSetting.json file, set the following environment variables:

DOCKER_REGISTRY: Your Docker registry (or leave blank for local builds).
TWILIO_SID, TWILIO_PHONE_NUMBER, and TWILIO_TOKEN: Set these if using Twilio for SMS notifications.
Docker Compose:
Run the following command to build and start the services:

```bash

docker-compose up --build
```
The service includes:

- electricitybillservice.api: The main API service for electricity bill operations.
- mysql: MySQL database container.
- localstack: LocalStack for simulating SNS and SQS messaging.
4. Access the API Documentation (Swagger UI):
Once the services are up, access the Swagger documentation at http://localhost:8080/swagger.

## Design Decisions
### 1. Modular Architecture
The service is structured with controllers, services, and repository patterns to promote modularity, maintainability, and testability. Key components are:

- Controllers: ElectricityController and WalletController for handling bill and wallet-related requests.
- Services: Handles the core logic, like bill creation, payment processing, and wallet management.
- Repositories: Manages data access for bills and wallets.
### 2. Event-Driven Design
The service leverages event-driven architecture, enabling decoupled communication between microservices and subsystems. Events like bill creation, successful payments, and fund additions are published to message queues.

### 3. Provider Abstraction for Payment Processing
The payment processing is abstracted using IElectricityProvider interface, allowing easy swapping of providers, represented by MockProviderA and MockProviderB.

## Event Handling and Messaging
1. Event Publishers
The BillService class publishes events using IEventPublisher to trigger actions in other parts of the system. Events published include:

- BillCreatedEvent: Triggered when a new bill is created.
- PaymentCompletedEvent: Triggered when a payment is successfully processed.
2. Messaging Configuration
The service uses LocalStack to simulate AWS SNS and SQS for local development. The following configuration is required in appsettings.json:

```json

"Messaging": {
    "Provider": "LocalStack",
    "ServiceURL": "http://localstack:4566",
    "SNSTopic": "arn:aws:sns:us-east-1:000000000000:ElectricityBillingEvents",
    "FundsAddedQueue": "FundsAddedQueue",
    "BillCreatedQueue": "BillCreatedQueue",
    "SuccessfulPaymentQueue": "SuccessfulPaymentQueue",
    "FundsAddedQueueUrl": "http://localstack:4566/000000000000/FundsAddedQueue",
    "BillCreatedQueueUrl": "http://localstack:4566/000000000000/BillCreatedQueue",
    "SuccessfulPaymentQueueUrl": "http://localstack:4566/000000000000/SuccessfulPaymentQueue"
}
````
3. LocalStack Initialization
SetupLocalStack class initializes SNS topics and SQS queues, and subscribes queues to the SNS topic for handling events.

## SMS Notification Configuration
The service is configured to send SMS notifications for key events such as bill creation and successful payment. SMS configuration is managed through the SmsSettings section in appsettings.json.
 
### Setting Up SMS Notifications    
1. Twilio SMS Provider
   To use Twilio for SMS notifications:
   - Obtain your Twilio Account SID, Auth Token, and Phone Number.
   - Update the SmsSettings in appsettings.json:
```json

"SmsSettings": {
    "BalanceThreshold": 10.0,
    "SmsProvider": "Twilio",
    "ApiKey": "mock-api-key",
    "TwilioSID": "<your-twilio-sid>",
    "TwilioPhoneNumber": "<your-twilio-phone-number>",
    "TwilioToken": "<your-twilio-auth-token>"
}
```
2. Threshold Notification
The BalanceThreshold setting determines the minimum wallet balance before sending a low balance alert. If the balance falls below this threshold, the service will send an SMS notification.
## Triggering Notifications
- SMS notifications are sent as part of the BillService logic when specific events are triggered.
- Notifications can be customized by modifying the ISmsService implementation.

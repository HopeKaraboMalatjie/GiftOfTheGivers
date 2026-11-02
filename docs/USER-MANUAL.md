# User Manual — Gift of the Givers Foundation prototype

This guide is for donors, volunteers, and staff using the academic prototype. The interface labels may vary slightly as the application evolves.

## Sign in or continue anonymously

- **Donor account:** Select **Log in** and enter the email address and password used at registration. New donors can select **Register** and complete the account form. Use account-recovery support from the site administrator if credentials are lost.
- **Anonymous donor:** No account is required to submit a donation. Open **Donate** and leave the guest name blank to record the donation as **Anonymous Donor**. An optional email can be provided for reference.
- **Employee:** Use the employee account provided by the system administrator. Only accounts assigned the Employee role can open the employee dashboard and publish project updates. Do not share employee credentials.

**Screenshot to insert:** Login page, with test data and personal details hidden.

## Submit a donation

1. Select **Donate**.
2. Enter an amount from 1 to 1,000,000.
3. Choose ZAR, USD, or EUR.
4. Choose one-time or recurring and, optionally, choose an active relief project.
5. For an anonymous donation, optionally enter a name and email; otherwise leave the name blank.
6. Select **Donate**. The confirmation page displays the amount and a certificate download link.

The recurring option records a frequency value in this prototype; it does **not** create a payment schedule or charge a payment method. The application does not currently collect or process payments.

**Screenshot to insert:** Completed donation form with test data only.

## Download a donation certificate

After submitting a donation, select **Download Certificate (PDF)** on the confirmation page. The PDF includes the donation reference, donor display name, date, amount, and selected project where available.

**Important:** The generated file is an academic demonstration/placeholder. It is not a legally valid SARS Section 18A tax certificate and must not be used for tax claims.

**Screenshot to insert:** Confirmation page and a redacted sample PDF showing the prototype disclaimer.

## Register as a volunteer

1. Select **Volunteer**.
2. Enter your full name and email address.
3. Optionally enter your phone number, skills/experience, and preferred active project.
4. Select an availability option: weekdays, weekends, full-time, or on-call/emergency only.
5. Select **Submit**. The confirmation page acknowledges that your interest was recorded.

Volunteer registration can be submitted without signing in. The application records the details for staff review; it does not automatically confirm placement.

**Screenshot to insert:** Volunteer form and confirmation page using test data only.

## Post an employee project update

1. Sign in with an Employee-role account.
2. Open **Employee Dashboard**.
3. Select **Post Project Update**.
4. Enter a title and description, optionally enter a location, and select a status (Active, Completed, or Suspended).
5. Select **Publish**. The project is saved and appears on the dashboard. The application also attempts to notify the Azure Function; a Function outage does not undo the saved project.

The dashboard shows relief projects, recent volunteer sign-ups, and a total of recorded donation amounts. It is not an accounting or payment settlement report.

**Screenshot to insert:** Employee dashboard and update form; hide volunteer personal information.

## Help and privacy

Use test data in demonstrations. Do not include real donor, volunteer, or employee personal data in screenshots, test runs, or public repositories. For access problems, contact the account administrator; for data correction or removal, contact the organization through its approved support channel.

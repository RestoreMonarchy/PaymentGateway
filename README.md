## PaymentGateway
This is a custom payment gateway for my projects.  
It's still under development and I will first implement it for my [UnturnedStore](https://github.com/RestoreMonarchy/UnturnedStore) project.

> *This is my payment gateway. There are many like it, but this one is mine.*  
> *My payment gateway is my best friend. It is my life. I must master it as I must master my life.*  
> *My payment gateway, without me, is useless. Without my payment gateway, I am useless...*

### Payment Gateway Client
```cs
PaymentGatewayClient client = new(new PaymentGatewayClientOptions()
{
    BaseAddress = "https://localhost:7255",
    APIKey = "be1ade6b-53ca-4d2c-8815-3127efbc4a8e"
});

Payment payment = Payment.Create(PaymentProviders.Mock, "YOUR_CUSTOM", "YOUR_RECEIVER", "USD", 49); 
// In case of mock, receiver can be null

payment.AddItem("Large Fries", 5, 2);
payment.AddItem("Cheeseburger", 2, 2);
payment.AddItem("Big Mac", 10, 3.5m);

Guid publicId = await client.CreatePaymentAsync(payment); // It sends a request to payment gateway web API 
string payUrl = client.BuildPayUrl(publicId); // Simply formats url: {PaymentGateway_URL}/pay/{publicId}
```
Now you can redirect user to `payUrl`. If he completes the payment notifications are going to be sent, to notify URL of the store, until your web API returns status code 200 (OK).

### Screenshots
<p float="left">
  <img src="https://i.imgur.com/FoxOdaD.png" width="384" height="216" />
  <img src="https://i.imgur.com/bCZCmRO.png" width="384" height="216" />
</p>

<p float="left">
  <img src="https://i.imgur.com/2gF0yAR.png" width="384" height="216" />
  <img src="https://i.imgur.com/EGOCtkR.jpeg" width="384" height="216" />
</p>

document.getElementById('donation-form').addEventListener('submit', async function (e) {
    e.preventDefault();
  
    const name = document.getElementById('donor-name').value;
    const amount = document.getElementById('donation-amount').value;
    const message = document.getElementById('donation-message').value;
  
    // Check if user is logged in (you'd replace this with your actual login check)
    const isLoggedIn = true; // Example: Replace with actual logic
  
    if (!isLoggedIn) {
      window.location.href = 'login.html';
      return;
    }
  
    // Send data to your backend
    const response = await fetch('/create-checkout-session', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ name, amount, message })
    });
  
    const data = await response.json();
  
    if (data.sessionId) {
      const stripe = Stripe('your-publishable-key'); // Replace with your Stripe key
      stripe.redirectToCheckout({ sessionId: data.sessionId });
    } else {
      alert('Error creating checkout session');
    }
  });
  
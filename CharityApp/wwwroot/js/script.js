document.addEventListener("DOMContentLoaded", () => {
  const menuToggle = document.querySelector(".menu-toggle");
  const navList = document.querySelector(".nav-list");

  menuToggle.addEventListener("click", () => {
    navList.classList.toggle("active"); // Toggle the active class
  });

  const navLinks = document.querySelectorAll(".nav-list a");
  navLinks.forEach(link => {
    link.addEventListener("click", () => {
      navList.classList.remove("active"); // Remove the active class when a link is clicked
    });
  });
});






  const toggleDarkMode = () => {
    document.body.classList.toggle('dark-mode');
    // Save the user's preference in local storage
    const isDarkMode = document.body.classList.contains('dark-mode');
    localStorage.setItem('darkMode', isDarkMode);
  };
  
  // Check for the user's saved preference on load
  document.addEventListener('DOMContentLoaded', () => {
    const savedDarkMode = localStorage.getItem('darkMode') === 'true';
    if (savedDarkMode) {
      document.body.classList.add('dark-mode');
    }
    document.getElementById('dark-mode-toggle').addEventListener('click', toggleDarkMode);
  });
  
  // Additional GSAP animation for entrance effects


 
  document.addEventListener("DOMContentLoaded", function () {
      gsap.from(".charity-container", { opacity: 0, y: 30, duration: 1.5, ease: "power2.out" });
  });

  //FAQ


  document.addEventListener("DOMContentLoaded", function() {
    const faqItems = document.querySelectorAll(".faq");

    faqItems.forEach(item => {
        const question = item.querySelector(".faq-question");
        
        question.addEventListener("click", function() {
            item.classList.toggle("active"); // Toggle the active class
        });
    });
});

//doner rating
document.addEventListener("DOMContentLoaded", function() {
  const donors = [
      { name: "Alice Johnson", amount: 500 },
      { name: "Bob Smith", amount: 450 },
      { name: "Charlie Lee", amount: 400 },
      { name: "David Kim", amount: 300 },
      { name: "Emily Davis", amount: 250 },
      { name: "Frank White", amount: 200 },
      { name: "Grace Hall", amount: 150 },
      { name: "Henry Scott", amount: 100 },
      { name: "Isabella Moore", amount: 75 },
      { name: "Jack Wilson", amount: 50 }
  ];

  // Sort donors by highest amount
  donors.sort((a, b) => b.amount - a.amount);

  const leaderboardBody = document.getElementById("leaderboard-body");

  // Populate the leaderboard table
  donors.forEach((donor, index) => {
      const row = document.createElement("tr");
      row.innerHTML = `
          <td>#${index + 1}</td>
          <td>${donor.name}</td>
          <td>$${donor.amount}</td>
      `;
      leaderboardBody.appendChild(row);
  });
});


//review by user


document.addEventListener("DOMContentLoaded", function () {
  const form = document.getElementById("feedback-form");
  const feedbackList = document.getElementById("feedback-items");
  const stars = document.querySelectorAll(".star");
  let selectedRating = 5;

  // Handle star rating
  stars.forEach(star => {
      star.addEventListener("click", function () {
          selectedRating = this.getAttribute("data-value");
          document.getElementById("rating").value = selectedRating;
          updateStars(selectedRating);
      });
  });

  function updateStars(rating) {
      stars.forEach(star => {
          star.classList.remove("selected");
          if (star.getAttribute("data-value") <= rating) {
              star.classList.add("selected");
          }
      });
  }

  // Handle feedback submission
  form.addEventListener("submit", function (event) {
      event.preventDefault();
      const name = document.getElementById("name").value;
      const message = document.getElementById("message").value;

      fetch("/submit-feedback", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ name, message, rating: selectedRating })
      })
      .then(response => response.json())
      .then(data => {
          if (data.success) {
              alert("Feedback submitted!");
              form.reset();
              updateStars(5);
              loadFeedback(); // Reload feedback list
          }
      });
  });

  // Load feedback from backend
  function loadFeedback() {
      fetch("/get-feedback")
      .then(response => response.json())
      .then(data => {
          feedbackList.innerHTML = "";
          data.forEach(feedback => {
              const listItem = document.createElement("li");
              listItem.innerHTML = `<strong>${feedback.name}:</strong> ${feedback.message} (⭐ ${feedback.rating})`;
              feedbackList.appendChild(listItem);
          });
      });
  }

  loadFeedback();
});





document.getElementById("feedback-form").addEventListener("submit", function(event) {
  event.preventDefault();
  
  const name = document.getElementById("name").value.trim();
  const email = document.getElementById("email").value.trim();
  const subject = document.getElementById("subject").value.trim();
  const message = document.getElementById("message").value.trim();

  if (email === "" || message === "") {
      alert("Please fill out the required fields.");
      return;
  }

  console.log("Feedback Submitted:", { name, email, subject, message });

  alert("Thank you for your feedback!");
  this.reset();
});



  
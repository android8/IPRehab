const data = [
  {
    "country": "USA",
    "state": "Texas",
    "population": "20M"
  },
  {
    "country": "USA",
    "state": "Alabama",
    "population": "4M"
  },
  {
    "country": "USA",
    "state": "California",
    "population": "33M"
  },
  {
    "country": "India",
    "state": "Maharashtra",
    "population": "112M"
  },
  {
    "country": "Japan",
    "state": "Tokyo",
    "population": "13M"
  },
  {
    "country": "India",
    "state": "Bihar",
    "population": "104M"
  },
  {
    "country": "USA",
    "state": "Florida",
    "population": "15M"
  },
  {
    "country": "India",
    "state": "Gujarat",
    "population": "60M"
  },
  {
    "country": "USA",
    "state": "Georgia",
    "population": "8M"
  },
  {
    "country": "Japan",
    "state": "Osaka",
    "population": "8M"
  },
  {
    "country": "Japan",
    "state": "Saitama",
    "population": "7M"
  },
  {
    "country": "India",
    "state": "Tamil Nadu",
    "population": "72M"
  }
];

$(() => {
  const countryArr = [... new Set(data.map(x => x.country))];

  countryArr.forEach((ele, index) => {
    $('#countryS').append(`<option value="${ele}">${ele}</option>`)
  });

  // After choosing country, showing the state
  $('#countryS').on('change', function () {
    // Reset the state array and drop down 
    var stateArr = [];
    $('#stateS').html(`<option value="">-- Select --</option>`);
    $('#showPopulation').val('');

    data.forEach((ele, idx) => {
      if (ele.country == this.value) {
        stateArr.push(ele.state);
      }
    }) // End of data forEach function
    stateArr.forEach((ele, index) => {
      $('#stateS').append(`<option value="${ele}">${ele}</option>`)
    });
  });

  // After choosing the state, show the population
  $('#stateS').on('change', function () {
    let country = $('#countryS').val();
    let state = $('#stateS').val();
    let population = data.filter(x => {
      return x.country == country && x.state == state;
    })[0]['population']

    //Parse the data to the input field
    $('#showPopulation').val(population);
  })

}) // End of onLoad function
  < script src = "https://cdnjs.cloudflare.com/ajax/libs/jquery/2.2.4/jquery.min.js" > </script>
    < body >
    <span>Country < /span>
    < select id = "countryS" >
      <option value="" > --Select --< /option>
        < /select>

        < span > State < /span>
        < select id = "stateS" >
          <option value="" > --Select --< /option>
            < /select>

            < input id = "showPopulation" placeholder = "Population" readonly />

              </body>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AlocacaoVeiuculo.Model.Map
{
 

public class LocalSugestao
{
    [JsonPropertyName("place_id")]
    public long PlaceId { get; set; }
         

        [JsonPropertyName("licence")]
    public string Licence { get; set; }

    [JsonPropertyName("osm_type")]
    public string OsmType { get; set; }

    [JsonPropertyName("osm_id")]
    public long OsmId { get; set; }

    [JsonPropertyName("lat")]
    public string Latitude { get; set; }

    [JsonPropertyName("lon")]
    public string Longitude { get; set; }

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }

    [JsonPropertyName("class")]
    public string Class { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("place_rank")]
    public int PlaceRank { get; set; }

    [JsonPropertyName("importance")]
    public double Importance { get; set; }

    [JsonPropertyName("boundingbox")]
    public string[] BoundingBox { get; set; }

    [JsonPropertyName("address")]
    public Address Address { get; set; }
}

public class Address
{
    [JsonPropertyName("house_number")]
    public string HouseNumber { get; set; }

    [JsonPropertyName("road")]
    public string Road { get; set; }

    [JsonPropertyName("suburb")]
    public string Suburb { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("municipality")]
    public string Municipality { get; set; }

    [JsonPropertyName("county")]
    public string County { get; set; }

    [JsonPropertyName("state_district")]
    public string StateDistrict { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("postcode")]
    public string Postcode { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("country_code")]
    public string CountryCode { get; set; }
}
}
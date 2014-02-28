using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lannion_Bus
{
    public class RoutingHelper
    {
        private string _applicationId;
        private Lannion_Bus.RouteService2.RouteServiceClient _routeService;
        
        int i = 0;

        public delegate void RouteCalculateEventHandler(MapLayer layer, LocationRect rect, string directions);
        public event RouteCalculateEventHandler RouteCalculated;
        public RoutingHelper(string applicationId)
        {
            _applicationId = applicationId;
            InitService();
        }

        private void InitService()
        {
            // Create the service variable and set the callback method using the CalculateRouteCompleted property.
            _routeService = new RouteService2.RouteServiceClient("BasicHttpBinding_IRouteService");
            _routeService.CalculateRouteCompleted -= new EventHandler<RouteService2.CalculateRouteCompletedEventArgs>(routeService2_CalculateRouteCompleted);
            _routeService.CalculateRouteCompleted += new EventHandler<RouteService2.CalculateRouteCompletedEventArgs>(routeService2_CalculateRouteCompleted);
        }

        public void CalculateRoute(List<Pushpin> wayPoints)
        {
            // Set the credentials.
            RouteService2.RouteRequest routeRequest = new RouteService2.RouteRequest();
            routeRequest.Credentials = new Credentials();
            routeRequest.Credentials.ApplicationId = _applicationId;

            // Return the route points so the route can be drawn.
            routeRequest.Options = new RouteService2.RouteOptions();
            routeRequest.Options.RoutePathType = RouteService2.RoutePathType.Points;

            // Set the waypoints of the route to be calculated using the Geocode Service results stored in the geocodeResults variable.
            routeRequest.Waypoints = new System.Collections.ObjectModel.ObservableCollection<RouteService2.Waypoint>();
            //Adding way points
            foreach (Pushpin ps in wayPoints)
            {
                routeRequest.Waypoints.Add(PushpinToWaypoint(ps));
            }

            // Make the CalculateRoute asnychronous request.
            _routeService.CalculateRouteAsync(routeRequest);
           
        }


        //Make the waypoints
        private RouteService2.Waypoint PushpinToWaypoint(Pushpin ps)
        {
            RouteService2.Waypoint waypoint = new RouteService2.Waypoint();
            waypoint.Description = "test";
            waypoint.Location = new Microsoft.Phone.Controls.Maps.Platform.Location();
            waypoint.Location = ps.Location;
            return waypoint;
        }

        // This is the callback method for the CalculateRoute request.
        private void routeService2_CalculateRouteCompleted(object sender, RouteService2.CalculateRouteCompletedEventArgs e)
        {
            try
            {
                // If the route calculate was a success and contains a route, then draw the route on the map.
                if ((e.Result.ResponseSummary.StatusCode == RouteService2.ResponseStatusCode.Success) & (e.Result.Result.Legs.Count != 0))
                {

                    // Add a map layer in which to draw the route.
                    MapLayer myRouteLayer = new MapLayer();
                    // Set properties of the route line you want to draw.

       

                    SolidColorBrush routeBrush = new SolidColorBrush(Colors.Red);
                 
                    MapPolyline routeLine = new MapPolyline();
                    routeLine.Locations = new LocationCollection();
                    routeLine.Stroke = routeBrush;
                    routeLine.Opacity = 0.65;
                    routeLine.StrokeThickness = 5.0;
                   
                    foreach (Location p in e.Result.Result.RoutePath.Points)
                    {
                        //waypint to the routeline
                        routeLine.Locations.Add(new GeoCoordinate(p.Latitude, p.Longitude));
                    }



                    // Add the route line to the new layer.
                    myRouteLayer.Children.Add(routeLine);
                    
                    // Figure the rectangle which encompasses the route. This is used later to set the map view.
                    // The rectangle must be defined as llx,lly, urx,ury. Since we don't know the exact route shape
                    // beforehand we need to validate the coordinates first
                    double minLat = routeLine.Locations[0].Latitude;
                    double minLon = routeLine.Locations[0].Longitude;
                    double maxLat = routeLine.Locations[routeLine.Locations.Count - 1].Latitude;
                    double maxLon = routeLine.Locations[routeLine.Locations.Count - 1].Longitude;

                    if (minLat > maxLat) // Swap min/max Lat
                    {
                        double tmpLat = 0;
                        tmpLat = minLat;
                        minLat = maxLat;
                        maxLat = tmpLat;
                    }
                    if (minLon > maxLon) // Swap min/max Lon
                    {
                        double tmpLon = 0;
                        tmpLon = minLon;
                        minLon = maxLon;
                        maxLon = tmpLon;
                    }
                    // This should always be a valid rectangle (minLat,minLon < maxLat,maxLon)
                    LocationRect rect = new LocationRect(minLat, minLon, maxLat, maxLon);

                    // Retrieve route directions from the RouteLeg.Itinerary class.
                    StringBuilder directions = new StringBuilder();

                    //Initialize directions counter
                    int instructionCount = 1;

                    //Loop through Interary instructions
                    foreach (var itineraryItem in e.Result.Result.Legs[0].Itinerary)
                    {
                        directions.Append(string.Format("{0}. {1} {2}\n",
                            instructionCount, itineraryItem.Summary.Distance, itineraryItem.Text));
                        instructionCount++;
                    }
                    // The itineraryItem.Text is in XML format e.g.:
                    //<VirtualEarth:Action>Depart</VirtualEarth:Action>
                    //<VirtualEarth:RoadName>Aigaiou</VirtualEarth:RoadName>
                    // toward 
                    // <VirtualEarth:Toward>Cheimonidou</VirtualEarth:Toward>
                    // In this sample we will remove all tags around keywords.  
                    // You could leave the tags on if you want to format the directions into a databound control
                    Regex regex = new Regex("<[/a-zA-Z:]*>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string results = regex.Replace(directions.ToString(), string.Empty);
                    
                    RouteCalculated(myRouteLayer, rect, results);
                    i++;
                  
                }
            }
            catch
            {
            }

        }
    }
}

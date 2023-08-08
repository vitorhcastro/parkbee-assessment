Feature: ParkingSession
Start and stop sessions

    Background:
        Given the users exist in the system
          | user   |
          | user 1 |
          | user 2 |
          | user 3 |
        And the garages exist in the system
          | garage   | totalSpots | doorHealth  |
          | garage 1 | 10         | Ok          |
          | garage 2 | 0          | Unreachable |
          | garage 3 | 0          | Unreachable |
          | garage 4 | 10         | Ok          |
          | garage 5 | 10         | Ok          |

    Scenario: Starting a new parking session for a user
        Given "user 1" has no running parking session
        And "garage 1" has available spots
        And location hardware is reachable
        When Start parking session API endpoint is called
        Then Endpoint should return a successful response with parking session id
        And Entry door should open

    Scenario: Starting a new parking session with no available spots in the garage
        Given "garage 2" has no parking spots available
        When Start parking session API endpoint is called
        Then Endpoint should return an error code
        And New parking session should not be created

    Scenario: Starting a new parking session when Entry door hardware is not reachable
        Given "garage 3" Entry door hardware is not reachable
        When Start parking session API endpoint is called
        Then Endpoint should return an error code
        And New parking session should not be created

    Scenario: Starting a new parking session for a user that already has a running parking session
        Given "user 2" has a running parking session in "garage 5"
        When Start parking session API endpoint is called
        Then Endpoint should return an error code
        And New parking session should not be created

    Scenario: Stopping running parking session
        Given parking session exists for "user 3" in "garage 4" and is running
        When Stop parking session API endpoint is called
        Then Endpoint returns a success code
        And Parking session should be stopped
        And Exit door should open

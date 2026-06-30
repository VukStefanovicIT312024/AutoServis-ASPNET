import { useEffect, useState } from "react";
import { Alert, Badge, Button, Card, Container, Spinner, Table } from "react-bootstrap";
import { Link } from "react-router-dom";
import { API_URL } from "../config";
import { useAuth } from "../context/AuthContext";

function AppointmentsPage() {
  const { user } = useAuth();

  const [appointments, setAppointments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    async function fetchAppointments() {
      if (!user) {
        setLoading(false);
        return;
      }

      try {
        setLoading(true);

        const response = await fetch(`${API_URL}/api/appointments`, {
          headers: {
            Authorization: `Bearer ${user.token}`,
          },
        });

        const data = await response.json();

        if (!response.ok) {
          throw new Error(data.message || "Greška pri učitavanju zakazivanja.");
        }

        setAppointments(data);
        setError("");
      } catch (error) {
        setError(error.message);
      } finally {
        setLoading(false);
      }
    }

    fetchAppointments();
  }, [user]);

  async function cancelAppointment(id) {
    try {
      const response = await fetch(`${API_URL}/api/appointments/${id}/cancel`, {
        method: "PUT",
        headers: {
          Authorization: `Bearer ${user.token}`,
        },
      });

      const data = await response.json();

      if (!response.ok) {
        throw new Error(data.message || "Otkazivanje nije uspelo.");
      }

      setAppointments(
        appointments.map((appointment) =>
          appointment._id === id ? { ...appointment, status: data.status } : appointment
        )
      );
    } catch (error) {
      setError(error.message);
    }
  }

  function getStatusLabel(status) {
    const labels = {
      zakazano: "Zakazano",
      u_obradi: "U obradi",
      zavrseno: "Završeno",
      otkazano: "Otkazano",
    };

    return labels[status] || status;
  }

  function getStatusVariant(status) {
    if (status === "zakazano") return "success";
    if (status === "u_obradi") return "warning";
    if (status === "zavrseno") return "primary";
    return "secondary";
  }

  if (!user) {
    return (
      <Container className="py-5">
        <Card className="info-card">
          <Card.Body>
            <p className="section-label">Zaštićena stranica</p>
            <h1>Moja zakazivanja</h1>
            <p>
              Da biste pregledali svoja zakazivanja, potrebno je da se prvo
              prijavite.
            </p>

            <Button as={Link} to="/prijava" variant="danger">
              Prijava
            </Button>
          </Card.Body>
        </Card>
      </Container>
    );
  }

  if (loading) {
    return (
      <Container className="py-5 text-center">
        <Spinner animation="border" variant="danger" />
        <p className="mt-3">Učitavanje zakazivanja...</p>
      </Container>
    );
  }

  return (
    <Container className="py-5">
      <div className="page-heading">
        <p className="section-label">Korisnički deo</p>
        <h1>Moja zakazivanja</h1>
        <p>Pregledajte zakazane termine i status svojih servisnih usluga.</p>
      </div>

      {error && <Alert variant="danger">{error}</Alert>}

      <Card className="table-card">
        <Card.Body>
          {appointments.length === 0 ? (
            <div>
              <p>Još uvek nemate zakazane termine.</p>

              <Button as={Link} to="/zakazivanje" variant="danger">
                Zakaži termin
              </Button>
            </div>
          ) : (
            <Table responsive hover>
              <thead>
                <tr>
                  <th>Usluga</th>
                  <th>Vozilo</th>
                  <th>Napomena</th>
                  <th>Datum</th>
                  <th>Vreme</th>
                  <th>Status</th>
                  <th></th>
                </tr>
              </thead>

              <tbody>
                {appointments.map((appointment) => (
                  <tr key={appointment._id}>
                    <td>{appointment.service?.name}</td>
                    <td>
                      {appointment.vehicle?.brand} {appointment.vehicle?.model}
                      <br />
                      <small>{appointment.vehicle?.plateNumber}</small>
                    </td>
                    <td>{appointment.description || "Nema napomene"}</td>
                    <td>{appointment.date}</td>
                    <td>{appointment.time}</td>
                    <td>
                      <Badge bg={getStatusVariant(appointment.status)}>
                        {getStatusLabel(appointment.status)}
                      </Badge>
                    </td>
                    <td className="text-end">
                      {appointment.status === "zakazano" && (
                        <Button
                          variant="outline-danger"
                          size="sm"
                          onClick={() => cancelAppointment(appointment._id)}
                        >
                          Otkaži
                        </Button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </Table>
          )}
        </Card.Body>
      </Card>
    </Container>
  );
}

export default AppointmentsPage;
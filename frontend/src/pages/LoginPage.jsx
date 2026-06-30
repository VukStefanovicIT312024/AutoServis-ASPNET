import { useState } from "react";
import { Alert, Button, Card, Col, Container, Form, Row } from "react-bootstrap";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const { login } = useAuth();
  const navigate = useNavigate();

  async function submitHandler(event) {
    event.preventDefault();

    if (!email || !password) {
      setError("Unesite email i lozinku.");
      return;
    }

    if (password.length < 6) {
      setError("Lozinka mora imati najmanje 6 karaktera.");
      return;
    }

    const result = await login(email, password);

if (!result.success) {
  setError(result.message);
  return;
}

navigate("/");
  }

  return (
    <Container className="py-5">
      <Row className="justify-content-center">
        <Col md={7} lg={5}>
          <Card className="auth-card">
            <Card.Body>
              <p className="section-label">Korisnički nalog</p>
              <h1 className="mb-4">Prijava</h1>

              {error && <Alert variant="danger">{error}</Alert>}

              <Form onSubmit={submitHandler}>
                <Form.Group className="mb-3" controlId="email">
                  <Form.Label>Email adresa</Form.Label>
                  <Form.Control
                    type="email"
                    placeholder="Unesite email"
                    value={email}
                    onChange={(event) => setEmail(event.target.value)}
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="password">
                  <Form.Label>Lozinka</Form.Label>
                  <Form.Control
                    type="password"
                    placeholder="Unesite lozinku"
                    value={password}
                    onChange={(event) => setPassword(event.target.value)}
                  />
                </Form.Group>

                <Button type="submit" variant="danger" className="w-100">
                  Prijavi se
                </Button>
              </Form>

              <p className="mt-4 mb-0 text-center">
                Nemate nalog?{" "}
                <Link to="/registracija">Registrujte se</Link>
              </p>

              <p className="auth-note mt-3">
                Demo administrator: admin@test.com / admin123
              </p>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
}

export default LoginPage;
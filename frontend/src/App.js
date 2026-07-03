import { useEffect, useState } from 'react';
import './App.css';

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:5000';

const formatDate = (dateString) => {
  const date = new Date(dateString);
  return date.toLocaleDateString(undefined, { year: 'numeric', month: 'short', day: 'numeric' });
};

function App() {
  const [formState, setFormState] = useState({ date: new Date().toISOString().split('T')[0], count: '' });
  const [deliveries, setDeliveries] = useState([]);
  const [summary, setSummary] = useState([]);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState('');

  const loadData = async () => {
    try {
      const [deliveriesRes, summaryRes] = await Promise.all([
        fetch(`${API_BASE_URL}/api/deliveries`),
        fetch(`${API_BASE_URL}/api/monthly-summary`),
      ]);

      if (!deliveriesRes.ok) {
        throw new Error('Failed to load daily entries.');
      }
      if (!summaryRes.ok) {
        throw new Error('Failed to load monthly totals.');
      }

      const deliveriesJson = await deliveriesRes.json();
      const summaryJson = await summaryRes.json();

      setDeliveries(deliveriesJson);
      setSummary(summaryJson);
    } catch (err) {
      console.error(err);
      setError('Unable to connect to the backend. Please make sure it is running.');
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleSubmit = async (event) => {
    event.preventDefault();
    setIsSubmitting(true);
    setError('');

    try {
      const response = await fetch(`${API_BASE_URL}/api/deliveries`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          date: formState.date,
          count: Number(formState.count || 0),
        }),
      });

      if (!response.ok) {
        const result = await response.json().catch(() => null);
        throw new Error(result?.message || 'Failed to add delivery.');
      }

      setFormState({ date: new Date().toISOString().split('T')[0], count: '' });
      await loadData();
    } catch (err) {
      console.error(err);
      setError(err.message);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="app-shell">
      <header className="hero">
        <div>
          <p className="eyebrow">Vendor delivery tracker</p>
          <h1>Daily Bottle Deliveries</h1>
          <p>
            Log how many drinking bottles the vendor delivers each day. The dashboard keeps an easy-to-read daily
            list plus a monthly summary so you can settle the vendor payment at month end.
          </p>
        </div>
        <div className="badge">No login needed</div>
      </header>

      <section className="card form-card">
        <h2>Add today's delivery</h2>
        <form className="delivery-form" onSubmit={handleSubmit}>
          <label>
            Delivery date
            <input
              type="date"
              value={formState.date}
              onChange={(event) => setFormState((prev) => ({ ...prev, date: event.target.value }))}
              required
            />
          </label>
          <label>
            Bottle count
            <input
              type="number"
              min="1"
              value={formState.count}
              onChange={(event) => setFormState((prev) => ({ ...prev, count: event.target.value }))}
              placeholder="e.g., 10"
              required
            />
          </label>
          <button type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Saving...' : 'Save entry'}
          </button>
        </form>
        {error && <p className="error">{error}</p>}
      </section>

      <section className="grid">
        <div className="card">
          <h2>Daily deliveries</h2>
          {deliveries.length === 0 ? (
            <p className="empty">No entries yet. Add today's count to get started.</p>
          ) : (
            <div className="table-wrapper">
              <table>
                <thead>
                  <tr>
                    <th>Date</th>
                    <th>Count</th>
                  </tr>
                </thead>
                <tbody>
                  {deliveries.map((item) => (
                    <tr key={item.id}>
                      <td>{formatDate(item.date)}</td>
                      <td>{item.count}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>

        <div className="card">
          <h2>Monthly summary</h2>
          {summary.length === 0 ? (
            <p className="empty">Monthly totals will appear here once you start logging deliveries.</p>
          ) : (
            <ul className="summary-list">
              {summary.map((entry) => (
                <li key={`${entry.year}-${entry.month}`}>
                  <div>
                    <strong>{new Date(entry.year, entry.month - 1, 1).toLocaleString(undefined, { month: 'long', year: 'numeric' })}</strong>
                    <span>{entry.total} bottles</span>
                  </div>
                </li>
              ))}
            </ul>
          )}
        </div>
      </section>
    </div>
  );
}

export default App;

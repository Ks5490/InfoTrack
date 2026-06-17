import { useRef, useState } from 'react'
import img from './assets/convey.jpg'
import './App.css'
import type { PotentialClientData } from './models/potentialClientData';
import { getPotentialClients } from './apis/gather-client-data';

const LOCATION_OPTIONS: Record<string, number> = {
  london: 0,
  birmingham: 1,
  leeds: 2,
  manchester: 3,
  sheffield: 4,
  bradford: 5,
  liverpool: 6,
  bristol: 7
};

function App() {
  const resultsRef = useRef<HTMLDivElement | null>(null);
  const [locations, setLocations] = useState<string[]>([''])
  const [enhancedSearch, setEnhancedSearch] = useState(false);
  const [loading, setLoading] = useState(false);
  const [showResults, setShowResults] = useState(false)
  const [reportData, setReportData] = useState<PotentialClientData[]>([])
  const [error, setError] = useState('')

  const updateLocation = (index: number, value: string) => {
    setLocations(prev => prev.map((loc, i) => i === index ? value : loc))
    setError('')
  }

  const addLocation = () => {
    if (locations.length < 9)
      setLocations(prev => [...prev, ''])
  }

  const removeLocation = (index: number) => {
    setLocations(prev => prev.filter((_, i) => i !== index))
  }

  const handleSearch = async () => {
    const filled = locations.filter(location => location.trim() !== '')
    if (filled.length === 0) {
      setError('At least one location is required.')
      return
    }

    setLoading(true);
    setError('');
    setShowResults(false);

    setTimeout(() => {
      resultsRef.current?.scrollIntoView({ behavior: "smooth", block: "start" });
    }, 0);

    try {
      const mappedLocations = filled.map(loc => Number(loc));
      const reportApiData = await getPotentialClients(mappedLocations, enhancedSearch);

      setReportData(reportApiData);
      setShowResults(true);
    }
    catch (err) {
      console.error(err);
      setError('Failed to fetch client data.');
    }
    finally {
      setLoading(false);
    }
  }

  const clearResults = () => {
    setLocations(['']);
    setReportData([]);
    setShowResults(false);
    setError('');
  };

  return (
    <>
      <section id="center">
        <div className="hero">
          <img src={img} className="base" width="400" height="230" alt="" />
        </div>

        <div>
          <h1>Looking to get some new clients?</h1>
          <p>Search by location to find potential clients. Add as many locations as you need using the dropdown below.</p>
          <p>*Please note enhanced search may take longer</p>
        </div>

        {error && <p className="error">{error}</p>}

        <div className="search-controls">
          <table className="locations-table">
            <thead>
              <tr>
                <th>#</th>
                <th>Location</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {locations.map((location, index) => (
                <tr key={index}>
                  <td>{index + 1}</td>
                  <td>
                    <select
                      id={`location-${index}`}
                      value={location}
                      onChange={(e) => updateLocation(index, e.target.value)}
                    >
                      <option value="">Select a location</option>

                      {Object.entries(LOCATION_OPTIONS).map(([key, value]) => {
                        const isSelectedElsewhere = locations.some(
                          (location, innerIndex) => innerIndex !== index && location === value.toString()
                        );
                        if (isSelectedElsewhere) return null;

                        return (
                          <option key={key} value={value}>
                            {key.charAt(0).toUpperCase() + key.slice(1)}
                          </option>
                        );
                      })}
                    </select>
                  </td>
                  <td>
                    {locations.length > 1 && (
                      <button
                        type="button"
                        className="remove-btn"
                        onClick={() => removeLocation(index)}
                        aria-label="Remove location"
                      >
                        X
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          <div className="search-actions">
            {locations.length < 9 && (
              <button type="button" className="add-btn" onClick={addLocation}>
                + Add location
              </button>
            )}
            <label className="enhanced-label">
              <input
                type="checkbox"
                checked={enhancedSearch}
                onChange={(e) => setEnhancedSearch(e.target.checked)}
              />
              Enhanced Search
            </label>
            <button type="button" className="searchbtn" onClick={handleSearch}>
              Search
            </button>
          </div>
        </div>

        <div ref={resultsRef}>
          {loading && (
            <div className="loader" />
          )}

          {showResults && (
            <div className="results">
              <button type="button" className="clearbtn" onClick={clearResults}>
                Clear Table
              </button>
              <table>
                <thead>
                  <tr>
                    <th>Organisation Name</th>
                    <th>Address</th>
                    <th>Website</th>
                    <th>Telephone</th>
                    <th>Total Reviews</th>
                    <th>Average Rating</th>
                  </tr>
                </thead>

                <tbody>
                  {reportData.map((row, i) => (
                    <tr key={i}>
                      <td>{row.name}</td>
                      <td>{row.address}</td>
                      <td>{row.websiteUrl}</td>
                      <td>{row.telephoneNumber}</td>
                      <td>{row.totalReviews == 0 ? 'N/A' : row.totalReviews}</td>
                      <td>{row.averageRating == 0 ? 'N/A' : row.averageRating}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </section>
    </>
  )
}

export default App
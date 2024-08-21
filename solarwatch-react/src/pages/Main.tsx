import {fetchSunriseSunset, ICityWithSunriseSunsetResponse} from "../utils/endpointHelpers.ts";
import React, {useState} from "react";

export const Main = () => {
    const [cityName, setCityName] = useState<string>('');
    const [results, setResults] = useState<ICityWithSunriseSunsetResponse[]>([]);
    const [error, setError] = useState<string | null>(null);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setCityName(e.target.value);
    };

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setError(null);

        try {
            const data = await fetchSunriseSunset(cityName);
            setResults(data);
        } catch (error) {
            console.error(error);
            setError('Failed to fetch sunrise and sunset data. Please try again.');
        }
    };

    return (
        <div className="main-container">
            <h2>Sunrise and Sunset Times</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="cityName">Enter City Name:</label>
                    <input
                        type="text"
                        id="cityName"
                        name="cityName"
                        value={cityName}
                        onChange={handleChange}
                        required
                    />
                </div>
                <button type="submit">Fetch Sunrise/Sunset Times</button>
            </form>

            {error && <p className="error">{error}</p>}

            <div className="results-container">
                {results.map((result, index) => (
                    <div key={index} className="result-item">
                        <h3>{result.cityName}, {result.state}, {result.country}</h3>
                        <p><strong>Latitude:</strong> {result.latitude}</p>
                        <p><strong>Longitude:</strong> {result.longitude}</p>
                        <p><strong>Sunrise:</strong> {result.sunrise}</p>
                        <p><strong>Sunset:</strong> {result.sunset}</p>
                    </div>
                ))}
            </div>
        </div>
    );
};
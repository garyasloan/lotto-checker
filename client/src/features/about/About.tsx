import React from "react";

const About: React.FC = () => {
  return (
    <div className="min-h-screen bg-white text-gray-900 p-6 flex flex-col items-center justify-center space-y-10">
      <section className="max-w-4xl w-full">
        <h1 className="text-4xl font-bold mb-6 text-center">This site uses .NET Core, REACT and Azure</h1>
        <ul className="list-disc list-inside text-xl space-y-8">
          <li style={{ marginBottom: "0.5rem" }}>
            <strong>"Serverless"</strong> Azure <strong>Container App</strong> hosted at: {" "}
            <div style={{ fontSize: '0.75rem', paddingLeft: '0rem' }}>
              https://lotto-checker-app.wittyglacier-91c7b4e8.westus2.
              <strong>azurecontainerapps</strong>.io
            </div>
          </li>
          <li style={{ marginBottom: "0.5rem" }}>
            <strong>.NET Core</strong> REST API
          </li>
          <li style={{ marginBottom: "0.5rem" }}>
            <strong>Entity Framework Core (EF Core)</strong> ORM
          </li>
          <li style={{ marginBottom: "0.5rem" }}>
            Azure-hosted <strong>SQL Server</strong>
          </li>
          <li style={{ marginBottom: "0.5rem" }}>
            <strong>React.js</strong> static single page application (<strong>SPA</strong>) served by the API
          </li>

          <li style={{ marginBottom: "0.5rem" }}>
            Styled with <strong>MUI (Material UI) </strong>
          </li>

          <li style={{ marginBottom: "0.5rem" }}>
            Uses the advanced <strong>DataGrid</strong> component from <strong>MUI X</strong>
          </li>

          <li style={{ marginBottom: "0.5rem" }}>
            Developed as a <strong>Docker container image</strong> using <strong>Docker Desktop</strong>
          </li>
          <li style={{ marginBottom: "0.5rem" }}>
            Production version built and deployed to <strong>Azure Container Apps</strong> via <strong>CI/CD</strong> using{" "}
            <strong>GitHub Actions</strong> in conjunction with <strong>GitHub Container Registry</strong>
          </li>
        </ul>
      </section>

      <section className="max-w-4xl w-full">
        <h2 className="text-3xl font-semibold mb-4 text-center">Website Description</h2>
        <p className="text-xl text-justify">
          Allows you to enter and save your super lotto picks and displays which draws resulted in
          cash prizes. <strong>The site was primarily built to showcase my technical work to prospective employers</strong> and replaces <s></s>mylottocheck.com which used older technology.
        </p>
      </section>

      <section className="max-w-4xl w-full">
        <h2 className="text-3xl font-semibold mb-4 text-center">Disclaimer</h2>
        <p className="text-xl text-center font-medium text-red-600">
          lotto-checker.com is not affiliated with any official lottery.  This web site is for entertainment purposes only.  You are ultimately responsible for checking your lottery ticket(s) at a lottery retailer in time to redeem any prizes due to you.
        </p>
      </section>

      <section className="max-w-4xl w-full">
        <h2 className="text-3xl font-semibold mb-4 text-center">Cookie Usage</h2>
        <p className="text-xl text-justify">
          This site uses a cookie as a key to store/retrieve picks into/from the database.
        </p>
      </section>
    </div>
  );
};

export default About;

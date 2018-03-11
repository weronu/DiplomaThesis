
USE XMLInvoices
GO
DECLARE @INVOICES XML
 
SET @INVOICES = '<Invoices>
    <Invoice>
        <InvoiceNumber>991001</InvoiceNumber>
        <InvoiceDate>2013-12-19</InvoiceDate>
            <InvoiceRows>
                <InvoiceRow>
                    <Product>PROD1234</Product>
                    <Amount>1000</Amount>
                </InvoiceRow>
                <InvoiceRow>
                    <Product>PROD5678</Product>
                    <Amount>100</Amount>
                </InvoiceRow>
				<InvoiceRow>
                    <Product>PROD5678</Product>
                    <Amount>100</Amount>
                </InvoiceRow>
				<InvoiceRow>
                    <Product>PROD5678</Product>
                    <Amount>100</Amount>
<InvoiceRow>
                    <Product>PROD5678</Product>
                    <Amount>100</Amount>
                </InvoiceRow>
				<InvoiceRow>
                    <Product>PROD5678</Product>
                    <Amount>100</Amount>
                </InvoiceRow>
				<InvoiceRow>
                    <Product>PROD5678</Product>
                    <Amount>100</Amount>
<InvoiceRow>
                    <Product>PROD5678</Product>
                    <Amount>100</Amount>
                </InvoiceRow>
				<InvoiceRow>
                    <Product>PROD5678</Product>
                    <Amount>100</Amount>
                </InvoiceRow>
                </InvoiceRow>
                </InvoiceRow>
            </InvoiceRows>
    </Invoice>
    <Invoice>
        <InvoiceNumber>991002</InvoiceNumber>
        <InvoiceDate>2013-12-20</InvoiceDate>
            <InvoiceRows>
                <InvoiceRow>
                    <Product>PROD1234</Product>
                    <Amount>2000</Amount>
                </InvoiceRow>
            </InvoiceRows>
    </Invoice>
    <Invoice>
        <InvoiceNumber>0</InvoiceNumber>
        <InvoiceDate>2013-12-01</InvoiceDate>
            <InvoiceRows>
                <InvoiceRow>
                    <Product>PRODXYX</Product>
                    <Amount>1000</Amount>
                </InvoiceRow>
            </InvoiceRows>
    </Invoice>
    <Invoice>
        <InvoiceNumber>0</InvoiceNumber>
        <InvoiceDate>2013-12-01</InvoiceDate>
            <InvoiceRows>
                <InvoiceRow>
                    <Product>PRODXYX2</Product>
                    <Amount>2000</Amount>
                </InvoiceRow>
            </InvoiceRows>
    </Invoice>
</Invoices>'
 
DECLARE @TMP_INVOICES TABLE
    (
        COL_InvoiceId INT NOT NULL
        ,COL_InvoiceNumber INT NOT NULL
        ,COL_InvoiceDate DATE NOT NULL
        ,COL_XMLPosition INT NOT NULL
    )
 
INSERT dbo.Invoices
    (
        InvoiceNumber
        ,InvoiceDate
        ,XMLPosition
    )
OUTPUT
    INSERTED.*
INTO
    @TMP_INVOICES
SELECT
    Invoice.InvoiceNode.value('InvoiceNumber[1]', 'INT') AS InvoiceNumber
    ,Invoice.InvoiceNode.value('InvoiceDate[1]', 'DATE') AS InvoiceDate
    ,DENSE_RANK() OVER (ORDER BY InvoiceNode) AS Position
FROM
    @INVOICES.nodes('/Invoices/Invoice') AS Invoice(InvoiceNode)
 
INSERT dbo.InvoiceRows
    (
        InvoiceId
        ,Product
        ,Amount
    )
SELECT
    T1.COL_InvoiceId
    ,Product
    ,Amount
FROM
    (
        SELECT
            DENSE_RANK() OVER (ORDER BY InvoiceNode) AS Position
            ,InvoiceRow.InvoiceRowNode.value('Product[1]', 'VARCHAR(10)') AS Product
            ,InvoiceRow.InvoiceRowNode.value('Amount[1]', 'INT') AS Amount
        FROM
            @INVOICES.nodes('/Invoices/Invoice') AS Invoice(InvoiceNode)
        CROSS APPLY
            Invoice.InvoiceNode.nodes('./InvoiceRows/InvoiceRow') AS InvoiceRow(InvoiceRowNode)
    ) AS T0
JOIN
    @TMP_INVOICES T1
ON
    T0.Position = T1.COL_XMLPosition